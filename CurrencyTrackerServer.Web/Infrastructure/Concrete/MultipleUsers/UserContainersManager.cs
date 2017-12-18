using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Workers;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace CurrencyTrackerServer.Web.Infrastructure.Concrete.MultipleUsers
{
  public class UserContainersManager
  {
    private readonly INotifier _notifier;
    private readonly UserContainerFactory _containerFactory;
    private Dictionary<string, AbstractUserMonitorsContainer> UserContainers { get; set; }
    private Dictionary<string, List<string>> TokenConnections { get; set; }

    private object _lockObject = new object();

    public UserContainersManager(INotifier notifier, UserContainerFactory containerFactory)
    {
      _notifier = notifier;
      _containerFactory = containerFactory;

      UserContainers = new Dictionary<string, AbstractUserMonitorsContainer>();
      TokenConnections = new Dictionary<string, List<string>>();
    }

    private async Task<string> GetOrCreateUserToken(string userId, UserManager<ApplicationUser> userManager)
    {
      var user = await userManager.FindByIdAsync(userId);
      if (user == null)
      {
        return string.Empty;
      }

      if (user.ConnectionToken == null)
      {
        user.ConnectionToken = Guid.NewGuid().ToString();
        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
          return string.Empty;
        }
      }
      return user.ConnectionToken;
    }


    /// <summary>
    /// Creates a user container and secret token if they don't exist. 
    /// Can be called periodically to ensure that user monitors are working
    /// and start them again if they don't
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="userManager">Instance of userManager to create or get a secret token</param>
    /// <returns>A secret token to be used to receive personal notifications</returns>
    /// User manager is passed here because it can't be injected into this singleton class
    public string InitializeUserContainer(string userId, UserManager<ApplicationUser> userManager)
    {
      lock (_lockObject) //Prevent creating multiple containers for one user
      {
        if (UserContainers.ContainsKey(userId))
        {
          var container = UserContainers[userId];

          if (container != null)
            return container.UserToken;
        }

        var token = GetOrCreateUserToken(userId, userManager).Result;
        if (string.IsNullOrEmpty(token))
        {
          return string.Empty;
        }

        var newContainer = _containerFactory.Create(userId);
        newContainer.UserToken = token;
        newContainer.ChangedCallback += SendToUser;

        Log.Warning("new user container " + userId);
        if (!UserContainers.TryAdd(userId, newContainer))
        {
          newContainer.Dispose();
          return string.Empty;
        }

        if (!TokenConnections.ContainsKey(token))
        {
          TokenConnections[token] = new List<string>();
        }
        return token;
      }

    }

    public AbstractUserMonitorsContainer GetUserContainer(string userId, UserManager<ApplicationUser> userManager)
    {
      var result = UserContainers.TryGetValue(userId, out var container);

      if (!result || container == null)
      {
        InitializeUserContainer(userId, userManager);
        UserContainers.TryGetValue(userId, out container);
      }

      return container;
    }

    public void SendToUser(string userToken, IEnumerable<BaseChangeEntity> changes)
    {
      var isConnected = TokenConnections.TryGetValue(userToken, out var connections);
      if (!isConnected)
      {
        return;
      }

      if (connections.Count == 0)
      {
        return;
      }

      var stillConnected = _notifier.SendToConnections(connections, changes);
      TokenConnections[userToken] = stillConnected;
    }

    public void AddConnection(string userToken, string connection)
    {
      if(string.IsNullOrEmpty(userToken) || string.IsNullOrEmpty(connection))
      {
        return;
      }

      if (!TokenConnections.ContainsKey(userToken))
      {
        TokenConnections.TryAdd(userToken, new List<string>());
      }

      var connections = TokenConnections[userToken];

      if (connections.Contains(connection))
      {
        return;
      }
      connections.Add(connection);

      TokenConnections[userToken] = connections;
    }
  }
}

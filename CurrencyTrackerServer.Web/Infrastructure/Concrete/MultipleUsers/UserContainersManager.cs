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

namespace CurrencyTrackerServer.Web.Infrastructure.Concrete.MultipleUsers
{
  public class UserContainersManager
  {
    private readonly INotifier _notifier;
    private readonly UserContainerFactory _containerFactory;
    private ConcurrentDictionary<string, AbstractUserMonitorsContainer> UserContainers { get; set; }
    private ConcurrentDictionary<string, List<string>> TokenConnections { get; set; }
    public UserContainersManager(INotifier notifier, UserContainerFactory containerFactory)
    {
      _notifier = notifier;
      _containerFactory = containerFactory;

      UserContainers = new ConcurrentDictionary<string, AbstractUserMonitorsContainer>();
      TokenConnections = new ConcurrentDictionary<string, List<string>>();
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
    public async Task<string> InitializeUserContainer(string userId, UserManager<ApplicationUser> userManager)
    {
      if (UserContainers.TryGetValue(userId, out var container) && container != null)
      {
        return container.UserToken;
      }

      var token = await GetOrCreateUserToken(userId, userManager);
      if (string.IsNullOrEmpty(token))
      {
        return string.Empty;
      }

      container = _containerFactory.Create(userId);
      container.UserToken = token;
      container.ChangedCallback += SendToUser;

      if (!UserContainers.TryAdd(userId, container))
      {
        return string.Empty;
      }

      if (!TokenConnections.ContainsKey(token))
      {
        if (!TokenConnections.TryAdd(token, new List<string>()))
        {
          return string.Empty;
        }
      }

      return token;
    }

    public AbstractUserMonitorsContainer GetUserContainer(string userId)
    {
      var result = UserContainers.TryGetValue(userId, out var container);
      return container;
    }

    public async void SendToUser(string userToken, IEnumerable<BaseChangeEntity> changes)
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

      var stillConnected = await _notifier.SendToConnections(connections, changes);
      TokenConnections.AddOrUpdate(userToken, stillConnected, (s, list) => stillConnected);
    }

    public void AddConnection(string userToken, string connection)
    {
      if (!TokenConnections.ContainsKey(userToken))
      {
        TokenConnections.TryAdd(userToken, new List<string>());
      }

      var connections = TokenConnections.GetValueOrDefault(userToken);

      if (connections.Contains(connection))
      {
        return;
      }
      connections.Add(connection);

      TokenConnections.TryUpdate(userToken, connections, null);
    }
  }
}

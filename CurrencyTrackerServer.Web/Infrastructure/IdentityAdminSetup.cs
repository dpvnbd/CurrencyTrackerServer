using CurrencyTrackerServer.Infrastructure.Entities.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyTrackerServer.Web.Infrastructure
{
  public class IdentityAdminSetup
  {
    public static void CreateRoles(IServiceProvider serviceProvider)
    {
      using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
      {

        //initializing custom roles 
        var RoleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var UserManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleNames = new List<string>();
        
        IdentityResult roleResult;

       
          var roleExist = RoleManager.RoleExistsAsync("Admin").Result;
          // ensure that the role does not exist
          if (!roleExist)
          {
            //create the roles and seed them to the database: 
            roleResult = RoleManager.CreateAsync(new IdentityRole("Admin")).Result;
          }
        

        // find the user with the admin email 
        var _user = UserManager.FindByNameAsync("admin").Result;

        // check if the user exists
        if (_user == null)
        {
          //Here you could create the super admin who will maintain the web app


          var poweruser = new ApplicationUser
          {
            UserName = "admin",
            Email = "admin@cts.pp.ua",
          };
          string adminPassword = "DefaultAdminPassword";

          var createPowerUser = UserManager.CreateAsync(poweruser, adminPassword).Result;
          if (createPowerUser.Succeeded)
          {
            //here we tie the new user to the role
            var rolesResult = UserManager.AddToRoleAsync(poweruser, "Admin").Result;
          }
        }
      }
    }
  }
}

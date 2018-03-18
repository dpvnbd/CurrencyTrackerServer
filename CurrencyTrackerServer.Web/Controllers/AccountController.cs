using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Entities.Data;
using CurrencyTrackerServer.Web.Infrastructure.Concrete.MultipleUsers;
using CurrencyTrackerServer.Web.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Collections.Generic;

namespace CurrencyTrackerServer.Web.Controllers
{
  [Route("api/[controller]")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  public class AccountController : Controller
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _config;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserContainersManager _containersManager;


    public AccountController(UserManager<ApplicationUser> userManager,
      IConfiguration config, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager,
      UserContainersManager containersManager)
    {
      _userManager = userManager;
      _config = config;
      _signInManager = signInManager;
      _roleManager = roleManager;
      _containersManager = containersManager;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
    {
      if (ModelState.IsValid)
      {
        var user = new ApplicationUser { UserName = model.Name, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
          Log.Information("User created a new account with password.");
          return Ok();
        }
        AddErrors(result);
      }

      // If we got this far, something failed
      return BadRequest(ModelState);
    }

    [HttpPost("changePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
    {
      if (ModelState.IsValid)
      {
        var user = await GetCurrentUser();
        if (user == null)
        {
          return BadRequest();
        }

        var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        if (result.Succeeded)
        {
          return Ok();
        }
        AddErrors(result);
      }

      // If we got this far, something failed
      return BadRequest(ModelState);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> GenerateToken([FromBody] LoginViewModel model)
    {
      if (ModelState.IsValid)
      {
        var user = await _userManager.FindByNameAsync(model.Name) ?? await _userManager.FindByEmailAsync(model.Name);

        if (user != null)
        {
          var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
          if (result.Succeeded)
          {
           
            var claims = new List<Claim>
            {
              new Claim(JwtRegisteredClaimNames.Sub, user.Email),
              new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
              claims.Add(new Claim(ClaimTypes.Role, userRole));
              var role = await _roleManager.FindByNameAsync(userRole);
              if (role != null)
              {
                var roleClaims = await _roleManager.GetClaimsAsync(role);
                foreach (Claim roleClaim in roleClaims)
                {
                  claims.Add(roleClaim);
                }
              }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TrackerTokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["TrackerTokens:Issuer"],
              _config["TrackerTokens:Issuer"],
              claims,
              expires: DateTime.Now.AddYears(1),
              signingCredentials: creds);

            var response = new {
              token = new JwtSecurityTokenHandler().WriteToken(token),
              email = user.Email,
              username = user.UserName,
              IsAdmin = _userManager.IsInRoleAsync(user, "Admin").Result,
              user.IsEnabled
            };

            return Ok(response);
          }
        }
      }
      return BadRequest("Could not create token");
    }

    [HttpPost("getToken")]
    public async Task<IActionResult> GetToken()
    {

      var user = await GetCurrentUser();
      if (user == null)
      {
        return BadRequest();
      }

      if (!user.IsEnabled)
      {
        return BadRequest(new {error = "Account is deactivated and can't receive updates. Ask admin to activate the account."});
      }

      var token = _containersManager.InitializeUserContainer(user.Id, _userManager);
      return Ok(new { token });
    }

    #region Helpers

    private void AddErrors(IdentityResult result)
    {
      foreach (var error in result.Errors)
      {
        ModelState.AddModelError(string.Empty, error.Description);
      }
    }

    private async Task<ApplicationUser> GetCurrentUser()
    {
      var email = User.FindFirst("sub")?.Value;
      return await _userManager.FindByEmailAsync(email);
    }
    #endregion
  }
}

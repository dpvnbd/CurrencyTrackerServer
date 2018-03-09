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

namespace CurrencyTrackerServer.Web.Controllers
{
  [Route("api/[controller]")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
  public class AdminController : Controller
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _config;


    public AdminController(UserManager<ApplicationUser> userManager,
      IConfiguration config, SignInManager<ApplicationUser> signInManager, UserContainersManager containersManager)
    {
      _userManager = userManager;
      _config = config;
    }

    [HttpGet("users")]
    public async Task<IActionResult> Users()
    {
      var users = _userManager.Users.ToArray();
      var response = users.Select(u => new
      {
        u.Id,
        u.Email,
        username = u.UserName,
        isAdmin = _userManager.IsInRoleAsync(u, "Admin").Result,
        u.IsEnabled
      });

      return Ok(response);
    }

    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
      var user = await _userManager.FindByIdAsync(id);
      var currentUser = await GetCurrentUser();
      if(user != null && currentUser.Id != id)
      {
        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
          return Ok();
        }
        AddErrors(result);
      }

      return BadRequest(ModelState);
    }

    [HttpPut("users/{id}")]
    public async Task<IActionResult> EnableUser(string id, [FromBody] UserEnableWrapper model)
    {
      var user = await _userManager.FindByIdAsync(id);
      if (user != null)
      {
        user.IsEnabled = model.IsEnabled;
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
          return Ok();
        }
        AddErrors(result);
      }

      return BadRequest(ModelState);
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

    public class UserEnableWrapper
    {
      public bool IsEnabled { get; set; }
    }
    #endregion
  }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Entities.Identity;
using CurrencyTrackerServer.Web.Models;
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
  public class AccountController : Controller
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _config;
    private readonly SignInManager<ApplicationUser> _signInManager;


    public AccountController(UserManager<ApplicationUser> userManager, IConfiguration config, SignInManager<ApplicationUser> signInManager)
    {
      _userManager = userManager;
      _config = config;
      _signInManager = signInManager;
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
            var claims = new[]
            {
              new Claim(JwtRegisteredClaimNames.Sub, user.Email),
              new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["TrackerTokens:Issuer"],
              _config["TrackerTokens:Issuer"],
              claims,
              expires: DateTime.Now.AddYears(1),
              signingCredentials: creds);

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
          }
        }
      }
      return BadRequest("Could not create token");
    }

    #region Helpers

    private void AddErrors(IdentityResult result)
    {
      foreach (var error in result.Errors)
      {
        ModelState.AddModelError(string.Empty, error.Description);
      }
    }

  
    #endregion
  }
}

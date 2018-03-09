using Microsoft.AspNetCore.Identity;

namespace CurrencyTrackerServer.Infrastructure.Entities.Data
{
  public class ApplicationUser : IdentityUser
  {
    public string ConnectionToken { get; set; }
    public bool IsEnabled { get; set; }
  }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyTrackerServer.Web.Models
{
  public class ChangePasswordViewModel
  {
    [Required]    
    [DataType(DataType.Password)]
    public string OldPassword { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
      MinimumLength = 4)]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }
  }
}

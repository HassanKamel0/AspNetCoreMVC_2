using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WoodyMovie.Resources;

namespace WoodyMovie.Models
{
    public class Login
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(SharedResource))]
        [EmailAddress(ErrorMessageResourceName = "Email", ErrorMessageResourceType = typeof(SharedResource))]
        [Display(Name="EmailLabel",ResourceType =typeof(SharedResource))]
        public string Email { get; set; }
        
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(SharedResource))]
        // [StringLength(maximumLength:30,MinimumLength =5)]
        public string Password { get; set; }
    }
}
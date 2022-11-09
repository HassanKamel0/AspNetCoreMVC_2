using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WoodyMovie.Resources;

namespace WoodyMovie.Models
{
    public class Register
    {
        [EmailAddress(ErrorMessageResourceName = "Email", ErrorMessageResourceType = typeof(SharedResource))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(SharedResource))]
        [Display(Name = "EmailLabel", ResourceType = typeof(SharedResource))]
        [Remote("IsEmailExists", "Account")]

        public string Email { get; set; }
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(SharedResource))]
        [StringLength(maximumLength: 30, MinimumLength = 5)]
        public string Password { get; set; }
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(SharedResource))]
        [Compare("Password")]

        public string ConfirmPassword { get; set; }
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(SharedResource))]
        [Range(9, 80)]

        public int Age { get; set; }
        //[RegularExpression(@"^/d{11}$")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(SharedResource))]

        public string PhoneNumber { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
    }
    public class ChangePasswordVM
    {
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        [StringLength(maximumLength: 30, MinimumLength = 5)]
        public string NewPassword { get; set; }
        [Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; }
    }
    public class AddPasswordVM
    {
        [Required]
        [DataType(DataType.Password)]
        [StringLength(maximumLength: 30, MinimumLength = 5)]
        public string Password { get; set; }

    }
    public class ConfirmEmailVM
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string UserID { get; set; }

    }
    
}

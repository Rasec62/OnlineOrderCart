using System.ComponentModel.DataAnnotations;

namespace OnlineOrderCart.Web.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "The field {0} is required.")]
        [Display(Name = "Email")]
        //[EmailAddress]
        public string Username { get; set; }

        [Required(ErrorMessage = "The field {0} is required.")]
        [Display(Name = "Password")]
        [StringLength(20, MinimumLength = 7, ErrorMessage = "The field {0} must contain between {2} and {1} characters.")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}

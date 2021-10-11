using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace OnlineOrderCart.Web.Models
{
    public class AddDistributorViewModel : EditDistributorViewModel
    {
        public string Username { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [MaxLength(100, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "The {0} field must contain between {2} and {1} characters.")]
        public string Password { get; set; }

        [Display(Name = "Password Confirm")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "The {0} field must contain between {2} and {1} characters.")]
        [Compare("Password")]
        public string PasswordConfirm { get; set; }

        [Display(Name = "First Name")]
        [MaxLength(50)]
        public string FirstName { get; set; }
        [Display(Name = "Last Name1")]
        [MaxLength(50)]
        public string LastName1 { get; set; }
        [Display(Name = "Last Name2")]
        [MaxLength(50)]
        public string LastName2 { get; set; }

        public string KamName { get; set; }

        [Display(Name = "First Name")]
        [MaxLength(50)]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public string EmployeeNumber { get; set; }
    }
}

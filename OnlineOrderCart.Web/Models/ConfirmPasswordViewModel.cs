using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.Models
{
    public class ConfirmPasswordViewModel
    {
        [Display(Name = "User Name")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "The {0} field must contain between {2} and {1} characters.")]
        public string UserName { get; set; }
        [Display(Name = "Current password")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 7, ErrorMessage = "The {0} field must contain between {2} and {1} characters.")]
        public string OldPassword { get; set; }

        [Display(Name = "New password")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "The {0} field must contain between {2} and {1} characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W]).{8,}$", ErrorMessage = "El {0} no cumple con los requisitos. Ejemplo(D*12345467a)")]
        public string NewPassword { get; set; }

        [Display(Name = "Password confirm")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "The {0} field must contain between {2} and {1} characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W]).{8,}$", ErrorMessage = "El {0} no cumple con los requisitos. Ejemplo(D*12345467a)")]
        [Compare("NewPassword")]
        public string Confirm { get; set; }
    }
}

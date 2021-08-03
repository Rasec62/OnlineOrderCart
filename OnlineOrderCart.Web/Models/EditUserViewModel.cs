using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.Models
{
    public class EditUserViewModel
    {
        public long UserId { get; set; }

        public long KamId { get; set; }
        [Display(Name = "First Name")]
        [MaxLength(50)]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name1")]
        [MaxLength(50)]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public string LastName1 { get; set; }
        [Display(Name = "Last Name2")]
        [MaxLength(50)]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public string LastName2 { get; set; }
        [Display(Name = "Email")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [MaxLength(100, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Employee Number")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [MaxLength(10, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string EmployeeNumber { get; set; }

        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }

        public Guid ImageId { get; set; }
        public string PicturePath { get; set; }
        public string PictureFPath { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Display(Name = "Gender")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a role.")]
        public int GenderId { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Display(Name = "Rol")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a role.")]
        public int RoleId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public long? KamManagerId { get; set; }

        public string CodeKey { get; set; }

        public IEnumerable<SelectListItem> ComboKams { get; set; }

        public IEnumerable<SelectListItem> ComboGender { get; set; }

        public IEnumerable<SelectListItem> ComboRoles { get; set; }
    }
}

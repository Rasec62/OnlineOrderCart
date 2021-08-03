using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineOrderCart.Web.Models
{
    public class EditDistributorViewModel
    {
        public long DistributorId { get; set; }
        [Display(Name = "Business Name")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [MaxLength(90, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string BusinessName { get; set; }
        
        [Display(Name = "Debtor")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Gender.")]
        public int Debtor { get; set; }
        
        [Display(Name = "MD")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [MaxLength(10, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string MD { get; set; }
        public int IsDeleted { get; set; }
        public DateTime RegistrationDate { get; set; }
        
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Display(Name = "Gender")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Gender.")]
        public int GenderId { get; set; }
        
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Display(Name = "Kam")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Kam.")]
        public long KamId { get; set; }
        
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Display(Name = "User")]
        public long UserId { get; set; }
        
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Display(Name = "Rol")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a role.")]
        public int RoleId { get; set; }

        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }

        public Guid ImageId { get; set; }

        public string PicturePath { get; set; }
        public string PictureFullPath { get; set; }

        public IEnumerable<SelectListItem> ComboGenders { get; set; }
        public IEnumerable<SelectListItem> ComboDisRoles { get; set; }
        public IEnumerable<SelectListItem> ComboKams { get; set; }
    }
}

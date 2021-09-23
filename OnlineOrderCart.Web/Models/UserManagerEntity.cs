using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineOrderCart.Web.Models
{
    public class UserManagerEntity
    {
        public long UserId { get; set; }
        public long KamId { get; set; }
        public string FirstName { get; set; }
        public string LastName1 { get; set; }
        public string LastName2 { get; set; }
        [Display(Name = "No Empleado")]
        public string EmployeeNumber { get; set; }
        public long? KamManagerId { get; set; }
        [Display(Name = "Usuario")]
        public string Username { get; set; }
        [Display(Name = "Rol")]
        public string RolName { get; set; }
        public string Email { get; set; }
        public Guid ImageId { get; set; }
        public string PicturePath { get; set; }
        public int GenderId { get; set; }
        public string Gender { get; set; }
        public int RolId { get; set; }
        public string CodeKey { get; set; }
        [Display(Name = "Es Admin?")]
        public bool IsAdmin { get; set; }

        public int IsCoordinator { get; set; }
        [Display(Name = "Kam")]
        public string KFullName { get; set; }
        [Display(Name = "Coordinador")]
        public string FullName => $"{FirstName} {LastName1} {LastName2}";
        [Display(Name = "Avatar")]
        public string ImageFullPath { get; set; }
        public string PictureFullPath { get; set; }
    }
}

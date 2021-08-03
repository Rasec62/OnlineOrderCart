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
        public string EmployeeNumber { get; set; }
        public long? KamManagerId { get; set; }
        public string Username { get; set; }
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
        public string FullName => $"{FirstName} {LastName1} {LastName2}";
        public string ImageFullPath { get; set; }
        public string PictureFullPath { get; set; }
    }
}

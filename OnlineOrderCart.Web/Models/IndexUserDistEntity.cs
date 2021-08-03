using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineOrderCart.Web.Models
{
    public class IndexUserDistEntity
    {
        public long UserId { get; set; }
        [Display(Name = "Distributor")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Range(1, int.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public long DistributorId { get; set; }
        public long KamId { get; set; }
        public int GenderId { get; set; }
        public int RolId { get; set; }
        public string FirstName { get; set; }
        public string LastName1 { get; set; }
        public string LastName2 { get; set; }
        public string EmployeeNumber { get; set; }
        public string Username { get; set; }
        public string RolName { get; set; }
        public string Email { get; set; }
        public Guid ImageId { get; set; }
        public string PicturePath { get; set; }
        public string PicturefullPath { get; set; }
        public string Gender { get; set; }
        public string BusinessName { get; set; }
        public string Debtor { get; set; }
        public string MD { get; set; }
        public string KFullName { get; set; }
        public int IsDistributor { get; set; }
        public string KFullNames => $"{FirstName} {LastName1} {LastName2}";
    }
}

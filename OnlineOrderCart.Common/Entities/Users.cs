using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineOrderCart.Common.Entities
{
    public class Users
    {
        [Key]
        public long UserId { get; set; }
        [Display(Name = "First Name")]
        [MaxLength(50)]
        //[Required(ErrorMessage = "The field {0} is mandatory.")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name1")]
        [MaxLength(50)]
        //[Required(ErrorMessage = "The field {0} is mandatory.")]
        public string LastName1 { get; set; }
        [Display(Name = "Last Name2")]
        [MaxLength(50)]
        //[Required(ErrorMessage = "The field {0} is mandatory.")]
        public string LastName2 { get; set; }
        [Display(Name = "Email")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [MaxLength(300, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [EmailAddress]
        public string Email { get; set; }
        [Display(Name = "Email")]
        [MaxLength(100, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string UserName { get; set; }
        public Guid ImageId { get; set; }

        public string PicturePath { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "The {0} field must contain between {2} and {1} characters.")]
        public string Password { get; set; }
        public string GenderId { get; set; }
        public int? IsDistributor { get; set; }
        public int IsDeleted { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string FullName => $"{FirstName} {LastName1}{LastName2}";
        public ICollection<Kams> GetKamsCollection { get; set; }
        public ICollection<Distributors> GetDistributorsCollection { get; set; }
        public ICollection<RoleGroups> GetRoleGroupUsersCollection { get; set; }
        public ICollection<PrOrders> GetPrOrders { get; set; }
        public ICollection<IncentiveOrders> GetsIncentiveOrders { get; set; }

        [Display(Name = "Image")]
        public string ImageFullPath
        {
            get
            {
                if (ImageId == Guid.Empty)
                {
                    //return $"https://onlinessales.azurewebsites.net/Image/noimage.png";
                    return $"https://localhost:44357/Image/noimage.png";
                }
                return $"https://onsalelayout.blob.core.windows.net/users/{ImageId}";
            }
        }

        //public string PictureFullPath
        //{
        //    get
        //    {
        //        if (PicturePath == null)
        //        {
        //            //return $"https://onlinessales.azurewebsites.net/Image/noimage.png";
        //            return $"https://localhost:44357/Image/noimage.png";
        //        }
        //        //return $"https://localhost:44357/Image/users/{ImageId}";
        //        return PicturePath;
        //    }
        //}

        public string PictureFullPath
        {
            get
            {
                if (PicturePath == null)
                {
                    return "http://shoppingcartsystems.ddns.net:8087/images/noimage.png";
                }

                return string.Format(
                    "http://shoppingcartsystems.ddns.net:8087{0}",
                    PicturePath.Substring(1));
            }
        }
    }
}

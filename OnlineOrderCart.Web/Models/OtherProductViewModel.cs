using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.Models
{
    public class OtherProductViewModel
    {
        [Display(Name = "Business Name")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public long StoreId { get; set; }
        public string ShippingBranchName { get; set; }
        [Display(Name = "Business Name")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [MaxLength(100, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string BusinessName { get; set; }
        [Display(Name = "Debtor")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [MaxLength(20, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string Debtor { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public long DeatilStoreId { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public long DistributorId { get; set; }

        [Display(Name = "Products")]
        [Required(ErrorMessage = "*")]
        [Range(1, int.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public int ProductId { get; set; }

        [Display(Name = "Purposes")]
        [Required(ErrorMessage = "*")]
        [Range(1, int.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public int PurposeId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public int SimTypeId { get; set; }

        public IEnumerable<SelectListItem> ComboProducts { get; set; }

        public IEnumerable<SelectListItem> ComboPurposes { get; set; }
    }
}

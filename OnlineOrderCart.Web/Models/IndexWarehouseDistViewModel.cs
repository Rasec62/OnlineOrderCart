using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineOrderCart.Common.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.Models
{
    public class IndexWarehouseDistViewModel
    {
        public long StoreId { get; set; }
        [Display(Name = "Shipping Branch No")]
        [Required(ErrorMessage = "*")]
        //[Range(1, int.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string ShippingBranchNo { get; set; }
        [Display(Name = "Shipping Branch Name")]
        [Required(ErrorMessage = "*")]
        [MaxLength(100, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string ShippingBranchName { get; set; }
        [Display(Name = "Sap Client")]
        [Required(ErrorMessage = "*")]
        [MaxLength(15, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string SapClient { get; set; }
        [Display(Name = "Sap Description")]
        [Required(ErrorMessage = "*")]
        [MaxLength(100, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string SapDescription { get; set; }
        [Display(Name = "Warehouse pvs")]
        [Required(ErrorMessage = "*")]
        [MaxLength(40, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string Warehousepvs { get; set; }
        [Display(Name = "StreetNumber")]
        [Required(ErrorMessage = "*")]
        [MaxLength(100, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string StreetNumber { get; set; }
        [Display(Name = "Suburd")]
        [Required(ErrorMessage = "*")]
        [MaxLength(100, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string Suburd { get; set; }
        [Display(Name = "Postal Code")]
        [Required(ErrorMessage = "*")]
        [MaxLength(5, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string PostalCode { get; set; }
        [Display(Name = "Delegation / Municipality")]
        [Required(ErrorMessage = "*")]
        [MaxLength(100, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string DelegationMunicipality { get; set; }
        [Display(Name = "State")]
        [Required(ErrorMessage = "*")]
        [MaxLength(100, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string State { get; set; }

        [DataType(DataType.MultilineText)]
        public string Observations { get; set; }

        [Display(Name = "Products")]
        [Required(ErrorMessage = "*")]
        [Range(1, int.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public int ProductId { get; set; }
        [Display(Name = "Purpose")]
        [Required(ErrorMessage = "*")]
        [Range(1, int.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public int PurposeId { get; set; }
        [Display(Name = "Distributor")]
        [Required(ErrorMessage = "*")]
        [Range(1, int.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public long DistributorId { get; set; }

        [Display(Name = "SimTypes")]
        [Required(ErrorMessage = "*")]
        [Range(1, int.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public int SimTypeId { get; set; }

        public IEnumerable<SelectListItem> ComboDistributors { get; set; }
        public IEnumerable<SelectListItem> ComboProducts { get; set; }
        public IEnumerable<SelectListItem> ComboSimTypes { get; set; }
        public IEnumerable<SelectListItem> ComboPurposes { get; set; }
        public List<Warehouses> DetailWarehousess { get; set; }

        public long KamId { get; set; }
        public long UserId { get; set; }
        public string BusinessName { get; set; }
        public string Debtor { get; set; }
        public string UserName { get; set; }
    }
}

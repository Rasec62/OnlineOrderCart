using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace OnlineOrderCart.Web.Models
{
    public class NOrderIncentiveViewModel
    {
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Range(1, int.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        //[Range(0.01, double.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Display(Name = "Price")]
        [Column(TypeName = "decimal(10,2)")]
        public double Price { get; set; }

        [Display(Name = "Value With Out Tax")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TaxPrice { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Range(1, int.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Display(Name = "Distributors")]
        public long DistributorId { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Range(0, int.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Display(Name = "Order Status")]
        public int OrderStatusId { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a warehouse")]
        [Display(Name = "Warehouses")]
        public int StoreId { get; set; }
        [Display(Name = "Products")]
        [Range(1, long.MaxValue, ErrorMessage = "You must select a product.")]
        public long DeatilStoreId { get; set; }

        [Range(1, long.MaxValue, ErrorMessage = "You must select a Type of Payment.")]
        [Required(ErrorMessage = "The field {0} is required")]
        [Display(Name = "Type of Payments")]
        public int TypeofPaymentId { get; set; }

        public string Debtor { get; set; }
        public int IncentiveId { get; set; }
        public IEnumerable<SelectListItem> CombosDistributors { get; set; }
        public IEnumerable<SelectListItem> CombosOrderStatuses { get; set; }
        public IEnumerable<SelectListItem> CombosWarehouses { get; set; }
        public IEnumerable<SelectListItem> CombosDWProducts { get; set; }
        public IEnumerable<SelectListItem> CombosTypeofPayments { get; set; }
        public List<TmpIncentiveDViewModel> DetailsTmp { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public double TotalQuantity { get { return DetailsTmp == null ? 0 : DetailsTmp.Sum(d => d.Quantity); } }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal TotalValue { get { return DetailsTmp == null ? 0 : DetailsTmp.Sum(d => d.Value); } }

    }
}

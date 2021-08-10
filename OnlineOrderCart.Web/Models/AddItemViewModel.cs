using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineOrderCart.Web.Models
{
    public class AddItemViewModel
    {
        [Range(0.0001, double.MaxValue, ErrorMessage = "The quantiy must be a positive number")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public int Debtor { get; set; }

        [DataType(DataType.MultilineText)]
        public string Observations { get; set; }

        public long DistributorId { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a warehouse")]
        [Display(Name = "Warehouses")]
        public int StoreId { get; set; }
        [Display(Name = "Products")]
        [Range(1, long.MaxValue, ErrorMessage = "You must select a product.")]
        public long DeatilStoreId { get; set; }
        public int OrderDetailTmpId { get; set; }
        [Range(1, long.MaxValue, ErrorMessage = "You must select a Type of Payment.")]
        [Required(ErrorMessage = "The field {0} is required")]
        [Display(Name = "Type of Payments")]
        public int TypeofPaymentId { get; set; }
        public IEnumerable<SelectListItem> CombosWarehouses { get; set; }
        public IEnumerable<SelectListItem> CombosDWProducts { get; set; }
        public IEnumerable<SelectListItem> CombosTypeofPayments { get; set; }
    }
}

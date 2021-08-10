using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineOrderCart.Common.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.Models
{
    public class NewOrderModel: PrOrders
    {
        [Display(Name = "Warehouse")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public string BusinessName { get; set; }
        public string KamName { get; set; }
       
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Range(1, int.MaxValue, ErrorMessage = "Usted debe de seleccionar un Products Type.")]
        public int OrderStatusId { get; set; }

        public string OrderCode { get; set; }

        public IEnumerable<SelectListItem> CombosOrderStatuses { get; set; }

        public List<TmpOrderViewModel> Details { get; set; }

        [DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]

        public double TotalQuantity { get { return Details == null ? 0 : Details.Sum(d => d.Quantity); } }

        [DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal TotalValue { get { return Details == null ? 0 : Details.Sum(d => d.Value); } }

    }
}

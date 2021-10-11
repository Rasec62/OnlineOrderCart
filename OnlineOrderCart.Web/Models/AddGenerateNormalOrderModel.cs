using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OnlineOrderCart.Web.Models
{
    public class AddGenerateNormalOrderModel: GenerateaNormalOrderViewModel
    {
        [Display(Name = "Estatus del Pedido")]
        public int OrderStatusId { get; set; }

        public long UserId { get; set; }

        [Display(Name = "Kam")]
        public string KamName { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Fecha de Orden")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
        public DateTime DateLocal => OrderDate.ToLocalTime();
        [Display(Name = "Kam Manager")]
        public long? KamManagerId { get; set; }

        public IEnumerable<SelectListItem> CombosOrderStatuses { get; set; }

        public List<TmpOrderViewModel> Details { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public double TotalQuantity { get { return Details == null ? 0 : Details.Sum(d => d.Quantity); } }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal TotalValue { get { return Details == null ? 0 : Details.Sum(d => d.Value); } }


    }
}

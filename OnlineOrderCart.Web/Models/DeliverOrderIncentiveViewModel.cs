using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OnlineOrderCart.Web.Models
{
    public class DeliverOrderIncentiveViewModel
    {
        public long UserId { get; set; }

        public long KamId { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Range(1, int.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Display(Name = "Distributors")]
        public long DistributorId { get; set; }
        public IEnumerable<SelectListItem> CombosDistributors { get; set; }
        [Display(Name = "Delivery date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime DeliveryDate { get; set; }
        [DataType(DataType.MultilineText)]
        [MaxLength(150, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public string Observations { get; set; }
        public List<TmpIncentiveDViewModel> DetailsTmp { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public double TotalQuantity { get { return DetailsTmp == null ? 0 : DetailsTmp.Sum(d => d.Quantity); } }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal TotalValue { get { return DetailsTmp == null ? 0 : DetailsTmp.Sum(d => d.Value); } }

    }
}

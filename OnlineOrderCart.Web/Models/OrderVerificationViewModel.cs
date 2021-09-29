using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OnlineOrderCart.Web.Models
{
    public class OrderVerificationViewModel
    {
        public long UserId { get; set; }

        [Display(Name = "Kams")]
        public long KamId { get; set; }
        public IEnumerable<SelectListItem> CombosKams { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Display(Name = "Distribuidor")]
        public long DistributorId { get; set; }
        public IEnumerable<SelectListItem> CombosDistributors { get; set; }

        [Display(Name = "Fecha de Verificacion.")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DeliveryDate { get; set; }

        [Display(Name = "Fecha de Orden")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}")]
        public DateTime DateLocal => DeliveryDate.ToLocalTime();

        [DataType(DataType.MultilineText)]
        [MaxLength(150, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Observaciones.")]
        public string Observations { get; set; }

        public List<TmpIncentiveDViewModel> DetailsTmp { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public double TotalQuantity { get { return DetailsTmp == null ? 0 : DetailsTmp.Sum(d => d.Quantity); } }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal TotalValue { get { return DetailsTmp == null ? 0 : DetailsTmp.Sum(d => d.Value); } }
    }
}

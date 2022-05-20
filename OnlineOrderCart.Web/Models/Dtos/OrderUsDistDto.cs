using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OnlineOrderCart.Web.Models.Dtos
{
    public class OrderUsDistDto
    {
        [Display(Name = "Orden Id")]
        public long OrdenId { get; set; }
        [Required(ErrorMessage = "The field {0} is required")]
        [Display(Name = "Deudor")]
        public string Deudor { get; set; }

        [Display(Name = "Status Orden")]
        public string OrderStatus { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Fecha Entrega")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}", ApplyFormatInEditMode = false)]
        public DateTime FechaEntrega { get; set; }
        [Display(Name = "Fecha de Orden")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
        public DateTime DateLocal => OrderDate.ToLocalTime();

        [Display(Name = "Operador")]
        [MaxLength(50)]
        //[Required(ErrorMessage = "The field {0} is mandatory.")]
        public string Operador { get; set; }

        [Display(Name = "Usuario Id")]
        public long UserId { get; set; }

        [Display(Name = "Distribuidor Id")]
        public long DistributorId { get; set; }
        [Display(Name = "Esta Borrado")]
        public int IsDeleted { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}")]
        public DateTime? RegistrationDate { get; set; }

        [Display(Name = "Fecha de entrega")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}")]
        public DateTime DDateLocal => FechaEntrega.ToLocalTime();
        

        [Display(Name = "Nombre del Negocio")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [MaxLength(100, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string BusinessName { get; set; }
        public List<OrderDetailDistDto> OrderDetailDist { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public double TotalQuantity { get { return OrderDetailDist == null ? 0 : OrderDetailDist.Sum(d => d.Quantity); } }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal TotalValue { get { return OrderDetailDist == null ? 0 : OrderDetailDist.Sum(d => d.Value); } }

    }
}

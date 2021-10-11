using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineOrderCart.Web.Models
{
    public class OnlyOrderDetails
    {
        [Display(Name = "Fecha de Pedido")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
        public DateTime? OrderDate { get; set; }
        [Display(Name = "No. de Deudor")]
        public string Debtor { get; set; }
        [Display(Name = "Nombre del cliente")]
        public string BusinessName { get; set; }
        [Display(Name = "Pedido")]
        public long OrderId { get; set; }

        [Display(Name = "Kam")]
        public string FullName { get; set; }
        [Display(Name = "Observaciones")]
        public string Observations { get; set; }
        [Display(Name = "Status")]
        public string OrderStatus { get; set; }
        [Display(Name = "Descripcion Corta")]
        public string ShortDescription { get; set; }
    }
}

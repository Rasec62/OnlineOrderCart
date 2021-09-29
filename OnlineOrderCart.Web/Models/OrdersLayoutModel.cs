using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineOrderCart.Web.Models
{
    public class OrdersLayoutModel
    {
        [Display(Name = "Fecha de Pedido")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm}")]
        public DateTime OrderDate { get; set; }
        [Display(Name = "No. de Deudor")]
        public string Debtor { get; set; }
        [Display(Name = "Nombre del cliente")]
        public string BusinessName { get; set; }
        [Display(Name = "No. de Sucursal de envío")]
        public string ShippingBranchNo { get; set; }
        [Display(Name = "Nombre sucursal de envío")]
        public string ShippingBranchName { get; set; }
        [Display(Name = "SKU")]
        public string OraclepId { get; set; }
        [Display(Name = "Descripción")]
        public string Description { get; set; }
        [Display(Name = "Cantidad")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Display(Name = "Codigo de Pedido")]
        public string OrderCode { get; set; }
        public string OrderStatus { get; set; }
        public string filePath { get; set; }
        public string To { get; set; }
        public long DeatilStoreId { get; set; }
        public long IncentiveOrderId { get; set; }
        public long IncentiveOrderDetailId { get; set; }
        [Display(Name = "METODO DE PAGO")]
        public string PaymentMethod { get; set; } = "99";
        [Display(Name = "USO CFDI")]
        public string UseCfdi { get; set; } = "G01";
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineOrderCart.Web.Models
{
    public class InOrderDetailViewModel
    {
        [Display(Name = "Fecha de Pedido")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Fecha de Entrega")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
        public DateTime? DeliveryDate { get; set; }

        public long OrderId { get; set; }
        public long OrderDetailId { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Display(Name = "No Deudor")]
        public string Debtor { get; set; }
        [Display(Name = "Nombre del cliente")]
        public string BusinessName { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Range(1, int.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Display(Name = "User")]
        public long UserId { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Range(1, int.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Display(Name = "Cantidad")]
        public int Quantity { get; set; }

        [Display(Name = "Precio")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [Display(Name = "Valor sin impuestos")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TaxPrice { get; set; }

        [Display(Name = "Tienda de detalles")]
        public long DeatilStoreId { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Range(1, double.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Display(Name = "Tipo de pago")]
        public int TypeofPaymentId { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Display(Name = "Estado del pedido")]
        public string OrderStatus { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Display(Name = "Código de pedido")]
        public string OrderCode { get; set; }

        [Display(Name = "Descripción de pedidos")]
        public string DeatilProducts { get; set; }

        [Display(Name = "Observaciones")]
        public string Observations { get; set; }
        public string MD { get; set; }
        [Display(Name = "Forma de pedidos")]
        public string PayofType { get; set; }
        [Display(Name = "No. de Sucursal de envío")]
        public string ShippingBranchNo { get; set; }
        [Display(Name = "Nombre sucursal de envío")]
        public string ShippingBranchName { get; set; }

        [Display(Name = "Descripcion Corta")]
        public string ShortDescription { get; set; }

        [Display(Name = "SKU")]
        public string OraclepId { get; set; }
        [Display(Name = "METODO DE PAGO")]
        public string PaymentMethod { get; set; } = "99";
        [Display(Name = "USO CFDI")]
        public string UseCfdi { get; set; } = "G01";
    }
}

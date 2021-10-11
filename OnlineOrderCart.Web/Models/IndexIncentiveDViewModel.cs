using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineOrderCart.Web.Models
{
    public class IndexIncentiveDViewModel
    {
        [Display(Name = "Fecha de Pedido")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
        public DateTime? OrderDate { get; set; }
        public long IncentiveOrderId { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Display(Name = "Debtor")]
        public string Debtor { get; set; }
        [Display(Name = "Nombre del cliente")]
        public string BusinessName { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Range(1, int.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Display(Name = "User")]
        public long UserId { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Range(1, int.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Range(1, double.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Display(Name = "Price")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Range(1, double.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Display(Name = "Value With Out Tax")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TaxPrice { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Range(1, double.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Display(Name = "Detail Store")]
        public long DeatilStoreId { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Range(1, double.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Display(Name = "Type of Payment")]
        public int TypeofPaymentId { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Display(Name = "Order Status")]
        public string OrderStatus { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Display(Name = "Order Code")]
        public string OrderCode { get; set; }
        public string DeatilProducts { get; set; }
        public string Observations { get; set; }
        public string MD { get; set; }
        public string PayofType { get; set; }
        [Display(Name = "No. de Sucursal de envío")]
        public string ShippingBranchNo { get; set; }
        [Display(Name = "Nombre sucursal de envío")]
        public string ShippingBranchName { get; set; }
        [Display(Name = "SKU")]
        public string OraclepId { get; set; }
        [Display(Name = "METODO DE PAGO")]
        public string PaymentMethod { get; set; } = "99";
        [Display(Name = "USO CFDI")]
        public string UseCfdi { get; set; } = "G01";
        [Display(Name = "Descripcion Corta")]
        public string ShortDescription { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineOrderCart.Common.Entities
{
    public class IncentiveOrderDetailTmp
    {
        [Key]
        public int IncentiveId { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Display(Name = "No de Deudor")]
        public string Debtor { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Range(1, int.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Display(Name = "Operador")]
        public long UserId { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Range(1, int.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Display(Name = "Cantidad")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Range(1, double.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Display(Name = "Precio")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Range(1, double.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Display(Name = "Valor sin impuestos")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TaxPrice { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Range(1, double.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Display(Name = "Producto")]
        public long DeatilStoreId { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Range(1, double.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Display(Name = "Tipo de Pago")]
        public int TypeofPaymentId { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Display(Name = "Status de la Orden")]
        public string OrderStatus { get; set; }

        [Display(Name = "Codigo de la Orden")]
        public string OrderCode { get; set; }
        [Display(Name = "Registration Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
        public DateTime? IODTDate { get; set; }
        [Display(Name = "Observaciones")]
        public string Observations { get; set; }
        public long DistributorId { get; set; }
    }
}

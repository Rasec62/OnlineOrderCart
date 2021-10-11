using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineOrderCart.Web.Models
{
    public class ObjectAvatarViewModel
    {
        [Display(Name = "Fecha de Pedido")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
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
        public string filePath { get; set; }
        public string To { get; set; }
        public long DeatilStoreId { get; set; }
        public long OrderId { get; set; }
        public long OrderDetailId { get; set; }
        [DataType(DataType.MultilineText)]
        public string Observations { get; set; }
        public string OrderStatus { get; set; }
        [Display(Name = "Tipo de Pago")]
        public string PayofType { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TaxPrice { get; set; }

        [Display(Name = "Descripción Corta")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string ShortDescription { get; set; }

        [Display(Name = "METODO DE PAGO")]
        public string PaymentMethod { get; set; } = "99";
        [Display(Name = "USO CFDI")]
        public string UseCfdi { get; set; } = "G01";
        public List<DKCEmailDetails> EmailDetails { get; set; }
        public DKCEmailDetails OptionalEmailDetails { get; set; }
    }
}

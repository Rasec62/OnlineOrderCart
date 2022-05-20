using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineOrderCart.Web.Models.Dtos
{
    public class OrderDetailDistDto
    {
        public long OrderDetailId { get; set; }
        [Required(ErrorMessage = "The field {0} is required")]
        public int Quantity { get; set; }
       
        [Column(TypeName = "decimal(10,2)")]
        [Required(ErrorMessage = "The field {0} is required")]
        [DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }
       
        [Column(TypeName = "decimal(10,2)")]
        [Required(ErrorMessage = "The field {0} is required")]
        [DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal TaxRate { get; set; }

        [DataType(DataType.MultilineText)]
        [MaxLength(50, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string Observations { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Display(Name = "Order Code")]
        [MaxLength(150, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string OrderCode { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        public long OrderId { get; set; }
        [Required(ErrorMessage = "The field {0} is required")]
        public long DeatilStoreId { get; set; }
        [Required(ErrorMessage = "The field {0} is required")]
        public int TypeofPaymentId { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Display(Name = "Order Status")]
        public string OrderStatus { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public long GenerateDistributor { get; set; }

        public int IsDeleted { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
        public DateTime? RegistrationDate { get; set; }

        [Display(Name = "Descripcion corta")]
        public string ShortDescription { get; set; }
        [Display(Name = "Payment")]
        public string PaymentName { get; set; }
        [Display(Name = "Producto")]
        public string Description { get; set; }

        [DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Value { get { return Price * (decimal)Quantity; } }
    }
}

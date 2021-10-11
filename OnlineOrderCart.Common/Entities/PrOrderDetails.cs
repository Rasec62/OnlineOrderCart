using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineOrderCart.Common.Entities
{
    public class PrOrderDetails
    {
        [Key]
        public long OrderDetailId { get; set; }
        [Required(ErrorMessage = "The field {0} is required")]
        public int Quantity { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Column(TypeName = "decimal(10,2)")]
        [Required(ErrorMessage = "The field {0} is required")]
        public decimal Price { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Column(TypeName = "decimal(10,2)")]
        [Required(ErrorMessage = "The field {0} is required")]
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

        [JsonIgnore]
        [ForeignKey("DeatilStoreId")]
        public virtual DeatilWarehouses DeatilWarehouses { get; set; }

        [JsonIgnore]
        [ForeignKey("OrderId")]
        public virtual PrOrders PrOrders { get; set; }
    }
}

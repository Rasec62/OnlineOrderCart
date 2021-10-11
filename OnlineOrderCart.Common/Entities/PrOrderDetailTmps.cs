using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineOrderCart.Common.Entities
{
    public class PrOrderDetailTmps
    {
        [Key]
        public int OrderDetailTmpId { get; set; }
        [Required(ErrorMessage = "The field {0} is required")]
        public string Debtor { get; set; }

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

        [MaxLength(50, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string Observations { get; set; }
        public string OrderCode { get; set; }
        //[NotMapped]
        [Required(ErrorMessage = "The field {0} is required")]
        public long DeatilStoreId { get; set; }
        
        public string OrderStatus { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        public int TypeofPaymentId { get; set; }

        public long? DistributorId { get; set; }
        public long? KamId { get; set; }
        public long? GenerateUserId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        public int GenerateDistributor { get; set; }

        [Display(Name = "Employee Number")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [MaxLength(10, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string EmployeeNumber { get; set; }

        [JsonIgnore]
        [ForeignKey("DeatilWarehouses")]
        [NotMapped]
        public virtual DeatilWarehouses DeatilWarehouses { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
        public DateTime OrderDate { get; set; }
        [Display(Name = "Fecha de Orden")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
        public DateTime DateLocal => OrderDate.ToLocalTime();

        [DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Value { get { return Price * (decimal)Quantity; } }

    }
}

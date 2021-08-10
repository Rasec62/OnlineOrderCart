using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineOrderCart.Common.Entities
{
    public class IncentiveOrderDetails
    {
        [Key]
        public long IncentiveOrderDetailId { get; set; }
        public int MyProperty { get; set; }

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
        [ForeignKey("DeatilWarehouses")]
        public long DeatilStoreId { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Range(1, double.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Display(Name = "Type of Payment")]
        [ForeignKey("TypeofPayments")] 
        public int TypeofPaymentId { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Display(Name = "Order Status")]
        public string OrderStatus { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Display(Name = "Order Code")]
        public string OrderCode { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Range(1, long.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Display(Name = "Distribuidor")]
        [ForeignKey("IncentiveOrders")]
        public long IncentiveOrderId { get; set; }
        public int IsDelete { get; set; }
        public DateTime RegistrationDate { get; set; }
        public IncentiveOrders IncentiveOrders { get; set; }
        public DeatilWarehouses DeatilWarehouses { get; set; }
        public TypeofPayments TypeofPayments { get; set; }
    }
}

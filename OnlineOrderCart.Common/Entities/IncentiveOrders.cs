using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineOrderCart.Common.Entities
{
    public class IncentiveOrders
    {
        [Key]
        public long IncentiveOrderId { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [MaxLength(15, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Display(Name = "Debtor")]
        public string Debtor { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Range(1, long.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Display(Name = "User")]
        [ForeignKey("Users")]
        public long UserId { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Range(1, long.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Display(Name = "Distribuidor")]
        [ForeignKey("Distributors")]
        public long DistributorId { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
        [Display(Name = "Incentive Date")]
        public DateTime OrderDate { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [MaxLength(350, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Obervations")]
        public string Observations { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Display(Name = "Order Status")]
        public string OrderStatus { get; set; }
        public int IsDeleted { get; set; }
        public DateTime RegistrationDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
        [Display(Name = "Send Date")]
        public DateTime? SendDate { get; set; }

        public Users Users { get; set; }
        public Distributors Distributors { get; set; }
        public ICollection<IncentiveOrderDetails> GetIncentiveOrderDetails { get; set; }
    }
}

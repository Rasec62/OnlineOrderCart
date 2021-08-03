using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineOrderCart.Common.Entities
{
    public class PrOrders
    {
        [Key]
        public long OrderId { get; set; }
        [Required(ErrorMessage = "The field {0} is required")]
        public string Debtor { get; set; }
        [DataType(DataType.MultilineText)]
        public string Observations { get; set; }
        public string OrderStatus { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm}")]
        public DateTime? OrderDate { get; set; }
        public long UserId { get; set; }
        public long DistributorId { get; set; }

        public int IsDeleted { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm}")]
        public DateTime? RegistrationDate { get; set; }

        [JsonIgnore]
        [ForeignKey("UserId")]
        public virtual Users Users { get; set; }

        [JsonIgnore]
        [ForeignKey("DistributorId")]
        public virtual Distributors Distributors { get; set; }

        public ICollection<PrOrderDetails> GetOrderDetails { get; set; }
    }
}

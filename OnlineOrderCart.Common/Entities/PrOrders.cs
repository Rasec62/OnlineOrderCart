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
        [Display(Name = "No Deudor")]
        public string Debtor { get; set; }

        [Display(Name = "Observaciones")]
        [Required(ErrorMessage = "The field {0} is required")]
        [DataType(DataType.MultilineText)]
        public string Observations { get; set; }
        public string OrderStatus { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm}")]
        public DateTime OrderDate { get; set; }
        [Display(Name = "Fecha de Orden")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}")]
        public DateTime DateLocal => OrderDate.ToLocalTime();

        [Display(Name = "Usuario")]
        public long UserId { get; set; }

        [Display(Name = "Distribuidor")]
        public long DistributorId { get; set; }

        public int IsDeleted { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm}")]
        public DateTime? RegistrationDate { get; set; }

        [Display(Name = "Delivery date")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}", ApplyFormatInEditMode = false)]
        public DateTime DeliveryDate { get; set; }

        [Display(Name = "Fecha de entrega")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}")]
        public DateTime DDateLocal => DeliveryDate.ToLocalTime();


        [JsonIgnore]
        [ForeignKey("UserId")]
        public virtual Users Users { get; set; }

        [JsonIgnore]
        [ForeignKey("DistributorId")]
        public virtual Distributors Distributors { get; set; }

        public ICollection<PrOrderDetails> GetOrderDetails { get; set; }
    }
}

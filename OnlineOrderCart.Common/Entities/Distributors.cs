using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineOrderCart.Common.Entities
{
    public class Distributors
    {
        [Key]
        public long DistributorId { get; set; }
        [Display(Name = "Business Name")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [MaxLength(100, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string BusinessName { get; set; }
        [Display(Name = "Debtor")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [MaxLength(20, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string Debtor { get; set; }
        [Display(Name = "MD")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [MaxLength(10, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string MD { get; set; }
        [ForeignKey("Kams")]
        public long KamId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime RegistrationDate { get; set; }
        [ForeignKey("Users")]
        public long UserId { get; set; }
        [JsonIgnore]
        public Users Users { get; set; }
        [JsonIgnore]
        public Kams Kams { get; set; }
        public ICollection<Warehouses> GetDistWarehouses { get; set; }
        public ICollection<PrOrders> GetOrders { get; set; }
        public ICollection<IncentiveOrders> GetIncentiveOrders { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace OnlineOrderCart.Common.Entities
{
    public class DeatilWarehouses
    {
        [Key]
        public long DeatilStoreId { get; set; }
        [ForeignKey("Warehouses")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public long StoreId { get; set; }
        [ForeignKey("Products")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public int ProductId { get; set; }
        [ForeignKey("Purposes")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public int PurposeId { get; set; }
        public int? IsDeleted { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm}")]
        public DateTime RegistrationDate { get; set; }

        [JsonIgnore]
        public Purposes Purposes { get; set; }
        [JsonIgnore]
        public Products Products { get; set; }
        [JsonIgnore]
        public Warehouses Warehouses { get; set; }
        public ICollection<PrOrderDetails> GetPrOrderDetails { get; set; }
        public ICollection<IncentiveOrderDetails> GetIncentiveOrderDetails { get; set; }
    }
}

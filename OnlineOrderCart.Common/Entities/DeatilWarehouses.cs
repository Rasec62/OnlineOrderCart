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
        public long StoreId { get; set; }
        [ForeignKey("Products")]
        public int ProductId { get; set; }
        [ForeignKey("Purposes")]
        public int PurposeId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime RegistrationDate { get; set; }

        [JsonIgnore]
        public Purposes Purposes { get; set; }
        [JsonIgnore]
        public Products Products { get; set; }
        [JsonIgnore]
        public Warehouses Warehouses { get; set; }
        public ICollection<PrOrderDetails> GetPrOrderDetails { get; set; }
    }
}

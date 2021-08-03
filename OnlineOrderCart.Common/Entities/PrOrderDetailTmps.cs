using Newtonsoft.Json;
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
        public long DeatilStoreId { get; set; }
        public string OrderStatus { get; set; }

        [JsonIgnore]
        [ForeignKey("DeatilWarehouses")]
        [NotMapped]
        public virtual DeatilWarehouses DeatilWarehouses { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Value { get { return Price * (decimal)Quantity; } }

    }
}

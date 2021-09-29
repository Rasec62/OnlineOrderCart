using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineOrderCart.Web.Models
{
    public class TmpOrderViewModel
    {
        public int OrderDetailTmpId { get; set; }
        public string Debtor { get; set; }
        public int Quantity { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TaxRate { get; set; }
        [MaxLength(50, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string Observations { get; set; }
        public long DeatilStoreId { get; set; }
        public string OrderStatus { get; set; }
        public string DeatilProducts { get; set; }
        public string OrderCode { get; set; }
        public string MD { get; set; }
        public string ShippingBranchNo { get; set; }
        public string PayofType { get; set; }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Value { get { return Price * (decimal)Quantity; } }
    }
}

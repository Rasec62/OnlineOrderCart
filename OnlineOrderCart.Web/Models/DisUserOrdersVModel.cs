using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineOrderCart.Web.Models
{
    public class DisUserOrdersVModel
    {
        [Display(Name = "Fecha de Pedido")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm}")]
        public DateTime OrderDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm}")]
        [Display(Name = "Order Date")]
        public DateTime DateLocal => OrderDate.ToLocalTime();
        public long OrderId { get; set; }
        public long IncentiveOrderId { get; set; }
        [Display(Name = "Debtor")]
        public string Debtor { get; set; }
        [Display(Name = "Nombre del cliente")]
        public string BusinessName { get; set; }
        public long UserId { get; set; }
        [DataType(DataType.MultilineText)]
        [Display(Name = "Obervations")]
        public string Observations { get; set; }
        [Display(Name = "Order Status")]
        public string OrderStatus { get; set; }
        public string FirstName { get; set; }
        public string LastName1 { get; set; }
        public string LastName2 { get; set; }

        [Display(Name = "Kam")]
        public string FullName => $"{FirstName} {LastName1} {LastName2}";
        public List<InOrderDetailViewModel> DetailsOrders { get; set; }
        public List<IndexIncentiveDViewModel> DetailsIncentive { get; set; }
    }
}

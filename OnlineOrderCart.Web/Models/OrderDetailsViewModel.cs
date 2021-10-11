using System.Collections.Generic;

namespace OnlineOrderCart.Web.Models
{
    public class OrderDetailsViewModel
    {
        public List<ObjectAvatarViewModel> OrdersDetails { get; set; }
        public List<OrdersLayoutModel> IncentiveOrdersDetails { get; set; }
    }
}

using OnlineOrderCart.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.Models
{
    public class IndexDWarehouseViewModel: Warehouses
    {
        public int ProductId { get; set; }
        public int PurposeId { get; set; }
    }
}

using OnlineOrderCart.Common.Responses;
using OnlineOrderCart.Web.Models;
using System.Collections.Generic;

namespace OnlineOrderCart.Web.Helpers
{
    public interface ICreateFileOrFolder
    {
        Response<object> WriteExcelFile(List<OrdersLayoutModel> OrdersDetails);
        Response<object> WriteExcelFileNormal(List<ObjectAvatarViewModel> OrdersNormalDetails);
    }
}

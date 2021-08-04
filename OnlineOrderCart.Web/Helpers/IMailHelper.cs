using OnlineOrderCart.Common.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.Helpers
{
    public interface IMailHelper
    {
        Response<object> SendMail(string to, string subject, string body);
    }
}

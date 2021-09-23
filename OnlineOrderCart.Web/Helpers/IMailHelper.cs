using OnlineOrderCart.Common.Responses;
using OnlineOrderCart.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.Helpers
{
    public interface IMailHelper
    {
        Response<object> SendMail(string to, string subject, string body);
        Response<object> SendMailAttachments(string to, string subject, string body, string FilePath);
        Response<object> SendMailToCAttachments(string to, List<DKCEmailDetails> toC, string subject, string body, string FilePath);
    }
}

using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using OnlineOrderCart.Common.Responses;
using OnlineOrderCart.Web.Models;
using System;
using System.Collections.Generic;

namespace OnlineOrderCart.Web.Helpers
{
    public class MailHelper : IMailHelper
    {
        private readonly IConfiguration _configuration;

        public MailHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Response<object> SendMail(string to, string subject, string body)
        {
            try
            {
                string from = _configuration["Mail:From"];
                string smtp = _configuration["Mail:Smtp"];
                string port = _configuration["Mail:Port"];
                string sSLPort = _configuration["Mail:SSLPort"];
                string password = _configuration["Mail:Password"];



                MimeMessage message = new MimeMessage();
                message.From.Add(new MailboxAddress(from));
                message.To.Add(new MailboxAddress(to));
                message.Subject = subject;

                BodyBuilder bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body
                };
                message.Body = bodyBuilder.ToMessageBody();

                using (SmtpClient client = new SmtpClient())
                {
                    client.Connect(smtp, int.Parse(port), SecureSocketOptions.None);
                    client.Authenticate(from, password);
                    client.Send(message);
                    client.Disconnect(true);
                }

                return new Response<object> { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new Response<object>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Result = ex
                };
            }
        }
        public Response<object> SendMailAttachments(string to, string subject, string body, string FilePath)
        {
            try
            {
                string from = _configuration["Mail:From"];
                string smtp = _configuration["Mail:Smtp"];
                string port = _configuration["Mail:Port"];
                string sSLPort = _configuration["Mail:SSLPort"];
                string password = _configuration["Mail:Password"];
                string MailOrders = _configuration["Mail:MailOrders"];



                MimeMessage message = new MimeMessage();
                message.From.Add(new MailboxAddress(from));
                message.To.Add(new MailboxAddress(MailOrders, to));
                message.Cc.Add(new MailboxAddress("marcos.nava@comtecom.com.mx", "marcosnavaramirez@gmail.com"));
                message.Subject = subject;

                BodyBuilder bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body
                };
                bodyBuilder.Attachments.Add(@FilePath, new ContentType("application", "xlsx"));

                message.Body = bodyBuilder.ToMessageBody();

                using (SmtpClient client = new SmtpClient())
                {
                    client.Connect(smtp, int.Parse(port), SecureSocketOptions.None);
                    client.Authenticate(from, password);
                    client.Send(message);
                    client.Disconnect(true);
                }
                return new Response<object> { IsSuccess = true };
            }
            catch (Exception exMail)
            {

                return new Response<object>
                {
                    IsSuccess = false,
                    Message = exMail.Message,
                    Result = exMail
                };
            }
        }
        public Response<object> SendMailToCAttachments(string to, List<DKCEmailDetails> EmailDetails, string subject, string body, string FilePath)
        {
            try
            {
                string from = _configuration["Mail:From"];
                string smtp = _configuration["Mail:Smtp"];
                string port = _configuration["Mail:Port"];
                string sSLPort = _configuration["Mail:SSLPort"];
                string password = _configuration["Mail:Password"];
                string MailOrders = _configuration["Mail:MailOrders"];



                MimeMessage message = new MimeMessage();
                message.From.Add(new MailboxAddress(from));
                message.To.Add(new MailboxAddress(MailOrders, to));

                foreach (var item2 in EmailDetails)
                {
                    if (string.IsNullOrEmpty(item2.EmailC) || item2.EmailC == null)
                    {
                        message.Cc.Add(new MailboxAddress(item2.EmailK, "developers.reptilian@outlook.com"));
                    }
                    else if (!string.IsNullOrEmpty(item2.EmailC) || item2.EmailC != null)
                    {
                        message.Cc.Add(new MailboxAddress(item2.EmailK, item2.EmailC));
                    }
                }

                message.Subject = subject;

                BodyBuilder bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body
                };
                bodyBuilder.Attachments.Add(@FilePath, new ContentType("application", "xlsx"));

                message.Body = bodyBuilder.ToMessageBody();

                using (SmtpClient client = new SmtpClient())
                {
                    client.Connect(smtp, int.Parse(port), SecureSocketOptions.None);
                    client.Authenticate(from, password);
                    client.Send(message);
                    client.Disconnect(true);
                }
                return new Response<object> { IsSuccess = true };
            }
            catch (Exception exMail)
            {

                return new Response<object>
                {
                    IsSuccess = false,
                    Message = exMail.Message,
                    Result = exMail
                };
            }
        }
        public Response<object> SendMailToOnlyCAttachments(string to, DKCEmailDetails toC, string subject, string body, string FilePath)
        {
            try
            {
                string from = _configuration["Mail:From"];
                string smtp = _configuration["Mail:Smtp"];
                string port = _configuration["Mail:Port"];
                string sSLPort = _configuration["Mail:SSLPort"];
                string password = _configuration["Mail:Password"];
                string MailOrders = _configuration["Mail:MailOrders"];



                MimeMessage message = new MimeMessage();
                message.From.Add(new MailboxAddress(from));
                message.To.Add(new MailboxAddress(MailOrders, to));

                if (!string.IsNullOrEmpty(toC.EmailC))
                {
                    message.Cc.Add(new MailboxAddress(toC.EmailK, toC.EmailC));
                }
                else {
                    message.Cc.Add(new MailboxAddress(toC.EmailK));
                }

                if (toC.OptionalEmailDetails.Count > 0){
                    foreach (var item in toC.OptionalEmailDetails){
                        message.Cc.Add(new MailboxAddress(item.OptionalEmail));
                    }
                }
                message.Subject = subject;

                BodyBuilder bodyBuilder = new BodyBuilder{
                    HtmlBody = body
                };
                bodyBuilder.Attachments.Add(@FilePath, new ContentType("application", "xlsx"));

                message.Body = bodyBuilder.ToMessageBody();

                using (SmtpClient client = new SmtpClient()){
                    client.Connect(smtp, int.Parse(port), SecureSocketOptions.None);
                    client.Authenticate(from, password);
                    client.Send(message);
                    client.Disconnect(true);
                }
                return new Response<object> { IsSuccess = true };
            }
            catch (Exception exMail)
            {

                return new Response<object>
                {
                    IsSuccess = false,
                    Message = exMail.Message,
                    Result = exMail
                };
            }
        }
    }
}

using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OnlineOrderCart.Common.Responses;
using OnlineOrderCart.Web.DataBase;
using OnlineOrderCart.Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace OnlineOrderCart.Web.Helpers
{
    public class CreateFileOrFolder : ICreateFileOrFolder
    {
        private readonly IConfiguration _configuration;
        private readonly IMailHelper _mailHelper;
        private readonly DataContext _dataContext;
        private readonly ICombosHelper _combosHelper;
        private string _Path = string.Empty;
        private List<ExcelDesignModel> OrdersLayouts;
        public CreateFileOrFolder(IConfiguration configuration
            , IMailHelper mailHelper, DataContext dataContext, ICombosHelper combosHelper){
            _configuration = configuration;
            _mailHelper = mailHelper;
            _dataContext = dataContext;
            _combosHelper = combosHelper;
        }
        public Response<object> WriteExcelFile(List<OrdersLayoutModel> OrdersDetails)
        {
            try{
                foreach (var items in OrdersDetails.OrderBy(o => o.IncentiveOrderId).ToList()) {
                }

                return new Response<object>
                {
                    IsSuccess = true,
                    Message = "Win Email...!",
                };
            }
            catch (Exception ex){
                bool result = File.Exists(_Path);
                if (result == true)
                {
                    Console.WriteLine("File Found Normal");
                    File.Delete(_Path);
                    Console.WriteLine("File Deleted Successfully");
                }
                else
                {
                    Console.WriteLine("File Not Found");
                }
                return new Response<object>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

        public Response<object> WriteExcelFileNormal(List<ObjectAvatarViewModel> OrdersNormalDetails)
        {
            try
            {
                foreach (var items in OrdersNormalDetails.OrderBy(o => o.OrderId).ToList()) {
                    OrdersLayouts = new List<ExcelDesignModel>();
                    var item = new ExcelDesignModel{
                        Fecha_de_Pedido = items.OrderDate,
                        No_de_Deudor = items.Debtor,
                        Nombre_del_Cliente = items.BusinessName,
                        No_de_Sucursal_de_envío = items.ShippingBranchNo,
                        Nombre_Sucursal_de_Envío = items.ShippingBranchName,
                        SKU = items.OraclepId,
                        Descripción = items.Description,
                        Cantidad = items.Quantity,
                        METODO_DE_PAGO = items.PaymentMethod,
                        USO_CFDI = items.UseCfdi,
                    };

                    OrdersLayouts.Add(item);
                    if (!Directory.Exists($"{_configuration["FilePath:SecretNormalFilePath"]}"))
                    {
                        DirectoryInfo di = Directory.CreateDirectory($"{_configuration["FilePath:SecretNormalFilePath"]}");
                    }
                    DataTable table = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(OrdersLayouts), (typeof(DataTable)));
                }

                return new Response<object>
                {
                    IsSuccess = true,
                    Message = "Win Email...!",
                };
            }
            catch (Exception ex)
            {
                bool result = File.Exists(_Path);
                if (result == true)
                {
                    Console.WriteLine("File Found Normal");
                    File.Delete(_Path);
                    Console.WriteLine("File Deleted Successfully");
                }
                else
                {
                    Console.WriteLine("File Not Found");
                }
                return new Response<object>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
    }
}

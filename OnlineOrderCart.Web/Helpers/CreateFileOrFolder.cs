using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Common.Responses;
using OnlineOrderCart.Web.DataBase;
using OnlineOrderCart.Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;

namespace OnlineOrderCart.Web.Helpers
{
    public class CreateFileOrFolder : ICreateFileOrFolder
    {
        private readonly IConfiguration _configuration;
        private readonly IMailHelper _mailHelper;
        private readonly DataContext _dataContext;
        private readonly ICombosHelper _combosHelper;
        private string _Path = string.Empty;
        private string filePath = string.Empty;
        private List<ExcelDesignModel> OrdersLayouts;
        public CreateFileOrFolder(IConfiguration configuration
            , IMailHelper mailHelper, DataContext dataContext, ICombosHelper combosHelper){
            _configuration = configuration;
            _mailHelper = mailHelper;
            _dataContext = dataContext;
            _combosHelper = combosHelper;
        }

        public Response<object> DWriteExcelFileGenerateReport(List<OrdersLayoutModel> OrdersDetails)
        {
            using (MemoryStream ms = new MemoryStream()) {
                try
                {
                    foreach (var items in OrdersDetails.OrderBy(o => o.IncentiveOrderId).ToList())
                    {
                        OrdersLayouts = new List<ExcelDesignModel>();
                        var item = new ExcelDesignModel
                        {
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
                        if (!Directory.Exists($"{_configuration["FilePath:SecretFilePath"]}"))
                        {
                            DirectoryInfo di = Directory.CreateDirectory($"{_configuration["FilePath:SecretFilePath"]}");
                        }
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        //ExcelPackage ep = new ExcelPackage();
                        filePath = $"{_configuration["FilePath:SecretNormalFilePath"]}{items.OrderCode}";
                        //Using SaveAs
                        using (ExcelPackage app = new ExcelPackage()) {
                            //create a new Worksheet
                            //process
                            app.Workbook.Worksheets.Add("Incentive hReport");
                            ExcelWorksheet ew1 = app.Workbook.Worksheets[0];

                            Excel.tituloHorizontal(ew1, "Layout Pedidos Distribuidores No Exclusivos", 1, 1, 10, 22);
                            Excel.anchosColumnas(ew1, 1, new List<int> { 25, 25, 40, 40, 45, 35, 45, 25, 25, 25 });
                            Excel.cabecerasTabla(ew1, 2, 1, new List<string> { "Fecha de Pedido", "No. de Deudor", "Nombre del cliente", "No. de Sucursal de envío", "Nombre sucursal de envío", "SKU", "Descripción", "Cantidad", "METODO DE PAGO", "USO CFDI" });

                        }
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

        public Response<object> WriteExcelFile(List<OrdersLayoutModel> OrdersDetails)
        {
            try{
                foreach (var items in OrdersDetails.OrderBy(o => o.IncentiveOrderId).ToList()) {
                    OrdersLayouts = new List<ExcelDesignModel>();
                    var item = new ExcelDesignModel
                    {
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
                    if (!Directory.Exists($"{_configuration["FilePath:SecretFilePath"]}"))
                    {
                        DirectoryInfo di = Directory.CreateDirectory($"{_configuration["FilePath:SecretFilePath"]}");
                    }
                    DataTable table = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(OrdersLayouts), (typeof(DataTable)));
                    using (SpreadsheetDocument document = SpreadsheetDocument.Create($"{_configuration["FilePath:SecretFilePath"]}{items.OrderCode}", SpreadsheetDocumentType.Workbook)) {
                        WorkbookPart workbookPart = document.AddWorkbookPart();
                        workbookPart.Workbook = new Workbook();

                        WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                        var sheetData = new SheetData();
                        worksheetPart.Worksheet = new Worksheet(sheetData);

                        Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                        Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet1" };

                        sheets.Append(sheet);

                        Row headerRow = new Row();

                        List<string> columns = new List<string>();
                        foreach (System.Data.DataColumn column in table.Columns)
                        {
                            columns.Add(column.ColumnName);

                            Cell cell = new Cell();
                            cell.DataType = CellValues.String;
                            cell.CellValue = new CellValue(column.ColumnName);
                            headerRow.AppendChild(cell);
                        }

                        sheetData.AppendChild(headerRow);

                        foreach (DataRow dsrow in table.Rows)
                        {
                            Row newRow = new Row();
                            foreach (String col in columns)
                            {
                                Cell cell = new Cell();
                                cell.DataType = CellValues.String;
                                cell.CellValue = new CellValue(dsrow[col].ToString());
                                newRow.AppendChild(cell);
                            }

                            sheetData.AppendChild(newRow);
                        }

                        workbookPart.Workbook.Save();
                    }
                    //TODO ENVIO DE CORREOS
                    string path = $"{_configuration["FilePath:SecretFilePath"]}{items.OrderCode}";
                    string body = $"Dear : Corresponding Area.";
                    body += $"{"<br /><br />the following email contains a file of the distributor's "}{items.BusinessName}";
                    body += $"{"<br /> incentive order"}{" Observations :"}{items.Observations}";
                    body += "<br /><br />Thanks";

                    var _Result = _mailHelper.SendMailToOnlyCAttachments(items.To, items.OptionalEmailDetails, "Realizar el proceso de pedido.", body, path);
                    if (_Result.IsSuccess) {
                        var _combo = _combosHelper
                            .GetOrderStatuses()
                            .Where(ou => ou.Value == "3")
                            .FirstOrDefault();

                        var _IResult = _dataContext.IncentiveOrders
                            .Where(i => i.IncentiveOrderId == items.IncentiveOrderId)
                            .FirstOrDefault();

                        if (_IResult != null)
                        {

                            _IResult.OrderStatus = _combo.Text;
                            _IResult.SendDate = DateTime.UtcNow;
                            _dataContext.IncentiveOrders.Update(_IResult);
                            _dataContext.SaveChanges();

                            var _DetailResult = _dataContext.IncentiveOrderDetails
                            .Where(io =>io.IncentiveOrderId.Equals(items.IncentiveOrderId))
                            .ToList();
                            List<IncentiveOrderDetails> IncentiveDetails = new List<IncentiveOrderDetails>();
                            foreach (var ditem in _DetailResult.ToList().OrderBy(x => x.IncentiveOrderDetailId))
                            {
                                IncentiveOrderDetails incentive = _dataContext
                                    .IncentiveOrderDetails.Where(d => d.IncentiveOrderDetailId.Equals(ditem.IncentiveOrderDetailId))
                                    .FirstOrDefault();
                                incentive.OrderStatus = _combo.Text;
                                _dataContext.IncentiveOrderDetails.Update(incentive);
                                _dataContext.SaveChanges();
                            }
                        }
                    }
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
                    using (SpreadsheetDocument document = SpreadsheetDocument.Create($"{_configuration["FilePath:SecretNormalFilePath"]}{items.OrderCode}", SpreadsheetDocumentType.Workbook))
                    {
                        WorkbookPart workbookPart = document.AddWorkbookPart();
                        workbookPart.Workbook = new Workbook();

                        WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                        var sheetData = new SheetData();
                        worksheetPart.Worksheet = new Worksheet(sheetData);

                        Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                        Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "NormalOrders" };

                        sheets.Append(sheet);

                        Row headerRow = new Row();

                        List<string> columns = new List<string>();
                        foreach (System.Data.DataColumn column in table.Columns)
                        {
                            columns.Add(column.ColumnName);

                            Cell cell = new Cell();
                            cell.DataType = CellValues.String;
                            cell.CellValue = new CellValue(column.ColumnName);
                            headerRow.AppendChild(cell);
                        }

                        sheetData.AppendChild(headerRow);

                        foreach (DataRow dsrow in table.Rows)
                        {
                            Row newRow = new Row();
                            foreach (String col in columns)
                            {
                                Cell cell = new Cell();
                                cell.DataType = CellValues.String;
                                cell.CellValue = new CellValue(dsrow[col].ToString());
                                newRow.AppendChild(cell);
                            }

                            sheetData.AppendChild(newRow);
                        }

                        workbookPart.Workbook.Save();
                    }
                    //TODO ENVIO DE CORREOS
                    string path = $"{_configuration["FilePath:SecretNormalFilePath"]}{items.OrderCode}";
                    string body = $"Hola : Área correspondiente.";
                    body += $"{"<br /><br />el siguiente correo electrónico contiene un archivo del pedido del distribuidor "}{items.BusinessName}";
                    body += $"{"<br /> Pedido normal"}";
                    body += "<br /><br />Gracias por su  fina atencion.";
                    var _Result = _mailHelper.SendMailToOnlyCAttachments(items.To, OrdersNormalDetails[0].OptionalEmailDetails, "Test Email.", body, path);
                    if (_Result.IsSuccess)
                    {
                        //ToDo okay Email;

                        var _combo = _combosHelper
                            .GetOrderStatuses()
                            .Where(ou => ou.Value == "3")
                            .FirstOrDefault();

                        PrOrders _IResultOrder = _dataContext.PrOrders
                            .Find(items.OrderId);

                        if (_IResultOrder != null)
                        {

                            _IResultOrder.OrderStatus = _combo.Text;
                            _IResultOrder.DeliveryDate = DateTime.UtcNow;
                            _dataContext.PrOrders.Update(_IResultOrder);
                            _dataContext.SaveChanges();

                            var _DetailResult = _dataContext.PrOrderDetails
                            .Where(io => io.DeatilStoreId.Equals(items.DeatilStoreId) &&
                            io.OrderId.Equals(items.OrderId) &&
                            io.OrderDetailId.Equals(items.OrderDetailId)).FirstOrDefault();

                            _DetailResult.OrderStatus = _combo.Text;
                            _dataContext.PrOrderDetails.Update(_DetailResult);
                            _dataContext.SaveChanges();
                        }
                    }
                    else
                    {
                        return new Response<object>
                        {
                            IsSuccess = false,
                            Message = _Result.Message,
                        };
                    }
                    bool result = File.Exists(path);
                    if (result == true)
                    {
                        Console.WriteLine("File Found Normal");
                        File.Delete(path);
                        Console.WriteLine("File Deleted Successfully");
                    }
                    else
                    {
                        Console.WriteLine("File Not Found");
                    }
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

        public Response<object> WriteExcelGenerateReport(List<ObjectAvatarViewModel> OrdersNormalDetails)
        {
            using (MemoryStream ms = new MemoryStream()) 
            {
                
                try
                {
                   
                    foreach (var items in OrdersNormalDetails.OrderBy(o => o.OrderId).ToList()) {
                        OrdersLayouts = new List<ExcelDesignModel>();
                        var item = new ExcelDesignModel
                        {
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
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        //ExcelPackage ep = new ExcelPackage();
                        filePath = $"{_configuration["FilePath:SecretNormalFilePath"]}{items.OrderCode}";
                        var Result = GenerateReport(OrdersLayouts,true, filePath);

                        if (Result.IsSuccess)
                        {
                            string body = $"Hola : Área correspondiente.";
                            body += $"{"<br /><br />el siguiente correo electrónico contiene un archivo del pedido del distribuidor "}{items.BusinessName}";
                            body += $"{"<br /> Pedido normal"}";
                            body += "<br /><br />Gracias por su  fina atencion.";
                            var _Result = _mailHelper.SendMailToOnlyCAttachments(items.To, OrdersNormalDetails[0].OptionalEmailDetails, "Test Email.", body, filePath);
                            if (_Result.IsSuccess)
                            {
                                //ToDo okay Email;

                                var _combo = _combosHelper
                                    .GetOrderStatuses()
                                    .Where(ou => ou.Value == "3")
                                    .FirstOrDefault();

                                PrOrders _IResultOrder = _dataContext.PrOrders
                                    .Find(items.OrderId);

                                if (_IResultOrder != null)
                                {

                                    _IResultOrder.OrderStatus = _combo.Text;
                                    _IResultOrder.DeliveryDate = DateTime.UtcNow;
                                    _dataContext.PrOrders.Update(_IResultOrder);
                                    _dataContext.SaveChanges();

                                    var _DetailResult = _dataContext.PrOrderDetails
                                    .Where(io => io.DeatilStoreId.Equals(items.DeatilStoreId) &&
                                    io.OrderId.Equals(items.OrderId) &&
                                    io.OrderDetailId.Equals(items.OrderDetailId)).FirstOrDefault();

                                    _DetailResult.OrderStatus = _combo.Text;
                                    _dataContext.PrOrderDetails.Update(_DetailResult);
                                    _dataContext.SaveChanges();
                                }
                            }
                            else
                            {
                                return new Response<object>
                                {
                                    IsSuccess = false,
                                    Message = _Result.Message,
                                };
                            }
                            bool result = File.Exists(filePath);
                            if (result == true)
                            {
                                Console.WriteLine("File Found Normal");
                                File.Delete(filePath);
                                Console.WriteLine("File Deleted Successfully");
                            }
                            else
                            {
                                Console.WriteLine("File Not Found");
                            }
                        }
                    }
                    //return Convert.ToBase64String(buffer);
                    return new Response<object>{
                        IsSuccess = true,
                        //Message = Convert.ToBase64String(buffer),
                    };
                }
                catch (Exception exception)
                {
                    return new Response<object>
                    {
                        IsSuccess = false,
                        Message = exception.InnerException.Message,
                    };
                }
            }
        }

        private Response<object> GenerateReport(List<ExcelDesignModel> OrdersLayouts, bool isNormal, string FilePath) {
            using (MemoryStream ms = new MemoryStream()) {
                try
                {
                    if (isNormal)
                    {
                        if (!Directory.Exists($"{_configuration["FilePath:SecretNormalFilePath"]}"))
                        {
                            DirectoryInfo di = Directory.CreateDirectory($"{_configuration["FilePath:SecretNormalFilePath"]}");
                        }
                        //filePath = $"{_configuration["FilePath:SecretNormalFilePath"]}{FilePath}";
                    }
                    else {
                        if (!Directory.Exists($"{_configuration["FilePath:SecretFilePath"]}"))
                        {
                            DirectoryInfo di = Directory.CreateDirectory($"{_configuration["FilePath:SecretFilePath"]}");
                        }
                        //filePath = $"{_configuration["FilePath:SecretNormalFilePath"]}{FilePath}";
                    }
                    
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    //ExcelPackage ep = new ExcelPackage();
                    

                    //Using SaveAs
                    using (ExcelPackage app = new ExcelPackage())
                    {
                        //create a new Worksheet
                        //process
                        if (isNormal)
                        {
                            app.Workbook.Worksheets.Add("Normal Report");
                        }
                        else {
                            app.Workbook.Worksheets.Add("Incentive Report");
                        }
                        
                        ExcelWorksheet ew1 = app.Workbook.Worksheets[0];

                        Excel.tituloHorizontal(ew1, "Layout Pedidos Distribuidores No Exclusivos", 1, 1, 10, 22);
                        Excel.anchosColumnas(ew1, 1, new List<int> { 25, 25, 40, 40, 45, 35, 45, 25, 25, 25 });
                        Excel.cabecerasTablas(ew1, 3, 1, new List<string> { "Fecha de Pedido", "No. de Deudor", "Nombre del cliente", "No. de Sucursal de envío", "Nombre Sucursal de envío", "SKU", "Descripción", "Cantidad", "METODO DE PAGO", "USO CFDI" });
                        //Excel.filasTabla<ExcelDesignModel>(ew1, OrdersLayouts, 4, 1, new List<string> { "Fecha_de_Pedido", "No_de_Deudor", "Nombre_del_cliente", "No_de_Sucursal_de_envío", "Nombre_Sucursal_de_Envío", "SKU", "Descripción", "Cantidad", "METODO_DE_PAGO", "USO_CFDI" });
                        int iniciofila = 4;
                        for (int i = 0; i < OrdersLayouts.Count; i++)
                        {
                            ew1.Cells[iniciofila, 1].Value = OrdersLayouts[i].DateLocal.ToString();
                            ew1.Cells[iniciofila, 1].Style.Font.Bold = true;
                            ew1.Cells[iniciofila, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            Excel.border(ew1, iniciofila, 1);
                            ew1.Cells[iniciofila, 2].Value = OrdersLayouts[i].No_de_Deudor;
                            ew1.Cells[iniciofila, 2].Style.Font.Bold = true;
                            ew1.Cells[iniciofila, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            Excel.border(ew1, iniciofila, 2);
                            ew1.Cells[iniciofila, 3].Value = OrdersLayouts[i].Nombre_del_Cliente;
                            ew1.Cells[iniciofila, 3].Style.Font.Bold = true;
                            ew1.Cells[iniciofila, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            Excel.border(ew1, iniciofila, 3);
                            ew1.Cells[iniciofila, 4].Value = OrdersLayouts[i].No_de_Sucursal_de_envío;
                            ew1.Cells[iniciofila, 4].Style.Font.Bold = true;
                            ew1.Cells[iniciofila, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            Excel.border(ew1, iniciofila, 4);
                            ew1.Cells[iniciofila, 5].Value = OrdersLayouts[i].Nombre_Sucursal_de_Envío;
                            ew1.Cells[iniciofila, 5].Style.Font.Bold = true;
                            ew1.Cells[iniciofila, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            Excel.border(ew1, iniciofila, 5);
                            ew1.Cells[iniciofila, 6].Value = OrdersLayouts[i].SKU;
                            ew1.Cells[iniciofila, 6].Style.Font.Bold = true;
                            ew1.Cells[iniciofila, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            Excel.border(ew1, iniciofila, 6);
                            ew1.Cells[iniciofila, 7].Value = OrdersLayouts[i].Descripción;
                            ew1.Cells[iniciofila, 7].Style.Font.Bold = true;
                            ew1.Cells[iniciofila, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            Excel.border(ew1, iniciofila, 7);
                            ew1.Cells[iniciofila, 8].Value = OrdersLayouts[i].Cantidad;
                            ew1.Cells[iniciofila, 8].Style.Font.Bold = true;
                            ew1.Cells[iniciofila, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            Excel.border(ew1, iniciofila, 8);
                            ew1.Cells[iniciofila, 9].Value = OrdersLayouts[i].METODO_DE_PAGO;
                            ew1.Cells[iniciofila, 9].Style.Font.Bold = true;
                            ew1.Cells[iniciofila, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            Excel.border(ew1, iniciofila, 9);
                            ew1.Cells[iniciofila, 10].Value = OrdersLayouts[i].USO_CFDI;
                            ew1.Cells[iniciofila, 10].Style.Font.Bold = true;
                            ew1.Cells[iniciofila, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            Excel.border(ew1, iniciofila, 10);

                            iniciofila++;
                        }

                        //the path of the file
                        //string filePath = $"{_configuration["FilePath:SecretNormalFilePath"]}{items.OrderCode}"; ;

                        //Write the file to the disk
                        FileInfo fi = new FileInfo(FilePath);
                        app.SaveAs(fi);
                        
                    }
                    return new Response<object> { IsSuccess =true };
                }
                catch (Exception exception)
                {
                    return new Response<object> { IsSuccess = false, Message= exception.InnerException.Message };
                }
            }
            
        }
    }
}

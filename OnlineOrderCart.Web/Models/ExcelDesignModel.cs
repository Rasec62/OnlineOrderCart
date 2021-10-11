using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineOrderCart.Web.Models
{
    public class ExcelDesignModel
    {
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
        public DateTime Fecha_de_Pedido { get; set; }
        [Display(Name = "No. de Deudor")]
        public string No_de_Deudor { get; set; }
        [Display(Name = "Nombre del cliente")]
        public string Nombre_del_Cliente { get; set; }
        [Display(Name = "No. de Sucursal de envío")]
        public string No_de_Sucursal_de_envío { get; set; }
        [Display(Name = "Nombre sucursal de envío")]
        public string Nombre_Sucursal_de_Envío { get; set; }
        [Display(Name = "SKU")]
        public string SKU { get; set; }
        [Display(Name = "Descripción")]
        public string Descripción { get; set; }
        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }

        [Display(Name = "METODO DE PAGO")]
        public string METODO_DE_PAGO { get; set; } = "99";
        [Display(Name = "USO CFDI")]
        public string USO_CFDI { get; set; } = "G01";
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace OnlineOrderCart.Web.Models
{
    public class NOrderIncentiveViewModel
    {
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Display(Name = "Cantidad")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        //[Range(0.01, double.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Display(Name = "Precio")]
        [Column(TypeName = "decimal(10,2)")]
        public double Price { get; set; }

        [Display(Name = "Valor sin impuestos")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TaxPrice { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Display(Name = "Distribuidor")]
        public long DistributorId { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Display(Name = "Status Pedido")]
        public int OrderStatusId { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar un almacén")]
        [Display(Name = "Almacen")]
        public int StoreId { get; set; }
        [Display(Name = "Producto")]
        [Range(1, long.MaxValue, ErrorMessage = "Debes seleccionar un Producto")]
        public long DeatilStoreId { get; set; }

        [Range(1, long.MaxValue, ErrorMessage = "Debes seleccionar un Tipo de Pago.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Tipo de Pago")]
        public int TypeofPaymentId { get; set; }

        public string Debtor { get; set; }
        public int IncentiveId { get; set; }
        public IEnumerable<SelectListItem> CombosDistributors { get; set; }
        public IEnumerable<SelectListItem> CombosOrderStatuses { get; set; }
        public IEnumerable<SelectListItem> CombosWarehouses { get; set; }
        public IEnumerable<SelectListItem> CombosDWProducts { get; set; }
        public IEnumerable<SelectListItem> CombosTypeofPayments { get; set; }
        public List<TmpIncentiveDViewModel> DetailsTmp { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public double TotalQuantity { get { return DetailsTmp == null ? 0 : DetailsTmp.Sum(d => d.Quantity); } }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal TotalValue { get { return DetailsTmp == null ? 0 : DetailsTmp.Sum(d => d.Value); } }

    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineOrderCart.Web.Models
{
    public class AddItemViewModel
    {
        [Display(Name = "Cantidad")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Range(0.0001, double.MaxValue, ErrorMessage = "La cantidad debe ser un número positivo")]
        public int Quantity { get; set; }

        public int Debtor { get; set; }

        [Display(Name = "Observaciones")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [DataType(DataType.MultilineText)]
        public string Observations { get; set; }

        [Display(Name = "Distribuidor")]
        [Range(1, long.MaxValue, ErrorMessage = "Debes seleccionar una Distribuidor.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public long DistributorId { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Range(1, long.MaxValue, ErrorMessage = "Debes seleccionar un Almacen.")]
        [Display(Name = "Almacen")]
        public int StoreId { get; set; }

        [Display(Name = "Producto")]
        [Range(1, long.MaxValue, ErrorMessage = "Debes seleccionar un Producto.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public long DeatilStoreId { get; set; }

        public int OrderDetailTmpId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar un Tipo de pago.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Tipo de Pago")]
        public int TypeofPaymentId { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public long UserId { get; set; }
        [Display(Name = "Kams")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public long KamId { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public int GenerateDistributor { get; set; }
        public string EmployeeNumber { get; set; }

        public IEnumerable<SelectListItem> CombosWarehouses { get; set; }
        public IEnumerable<SelectListItem> CombosDWProducts { get; set; }
        public IEnumerable<SelectListItem> CombosTypeofPayments { get; set; }
    }
}

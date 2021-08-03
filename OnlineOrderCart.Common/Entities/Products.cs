using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineOrderCart.Common.Entities
{
    public class Products
    {
        [Key]
        public int ProductId { get; set; }
        [Display(Name = "Description")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Description { get; set; }
        [Display(Name = "CodeKey")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        //[Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string CodeKey { get; set; }
        [Display(Name = "Value With Out Tax")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Range(1, double.MaxValue, ErrorMessage = "Usted debe de seleccionar un Tax.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal ValueWithOutTax { get; set; }
        [Display(Name = "Price")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Range(1, double.MaxValue, ErrorMessage = "Usted debe de seleccionar un Price.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
        [Display(Name = "Units In Stock")]
        [Range(1, int.MaxValue, ErrorMessage = "Usted debe de seleccionar un Units In Stock.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public int UnitsInStock { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Usted debe de seleccionar un Trade marks.")]
        [Display(Name = "Trade marks")]
        [ForeignKey("Trademarks")]
        public int TrademarkId { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Usted debe de seleccionar un Products Type.")]
        [Display(Name = "Products Type")]
        [ForeignKey("ProductsType")]
        public int ProductTypeId { get; set; }
        public Guid ImageId { get; set; }
        public int IsDeleted { get; set; }

        [Display(Name = "SKU")]
        [MaxLength(70, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string OraclepId { get; set; }

        [ForeignKey("SimTypes")]
        [Range(1, int.MaxValue, ErrorMessage = "Usted debe de seleccionar un Products Type.")]
        public int SimTypeId { get; set; }
        [ForeignKey("ActivationsForm")]
        [Range(1, int.MaxValue, ErrorMessage = "Usted debe de seleccionar un Products Type.")]
        public int ActivationFormId { get; set; }
        [ForeignKey("ActivationsType")]
        [Range(1, int.MaxValue, ErrorMessage = "Usted debe de seleccionar un Products Type.")]
        public int ActivationTypeId { get; set; }
        public DateTime RegistrationDate { get; set; }
        [JsonIgnore]
        public Trademarks Trademarks { get; set; }
        [JsonIgnore]
        public ProductsType ProductsType { get; set; }

        [JsonIgnore]
        public SimTypes SimTypes { get; set; }

        [JsonIgnore]
        public ActivationsForm ActivationsForm { get; set; }

        [JsonIgnore]
        public ActivationsType ActivationsType { get; set; }
        public ICollection<DeatilWarehouses> GetProdDWarehouses { get; set; }

    }
}

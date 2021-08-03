using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineOrderCart.Common.Entities
{
    public class Warehouses
    {
        [Key]
        public long StoreId { get; set; }
        [Display(Name = "Business Name")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        //[Range(1, int.MaxValue, ErrorMessage = "Usted debe de seleccionar un Products Type.")]
        public string ShippingBranchNo { get; set; }
        [Display(Name = "Business Name")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [MaxLength(100, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string ShippingBranchName { get; set; }
        [Display(Name = "Business Name")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [MaxLength(15, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string SapClient { get; set; }
        [Display(Name = "Business Name")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [MaxLength(100, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string SapDescription { get; set; }
        [Display(Name = "Business Name")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [MaxLength(40, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string Warehousepvs { get; set; }
        [Display(Name = "Business Name")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [MaxLength(100, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string StreetNumber { get; set; }
        [Display(Name = "Business Name")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [MaxLength(100, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string Suburd { get; set; }
        [Display(Name = "Business Name")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [MaxLength(5, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string PostalCode { get; set; }
        [Display(Name = "Business Name")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [MaxLength(100, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string DelegationMunicipality { get; set; }
        [Display(Name = "Business Name")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [MaxLength(100, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string State { get; set; }

        [Display(Name = "Observations")]
        [DataType(DataType.MultilineText)]
        [MaxLength(500, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string Observations { get; set; }

        public int SimTypeId { get; set; }

        [ForeignKey("Distributors")]
        public long DistributorId { get; set; }

        public int IsDeleted { get; set; }
        public DateTime RegistrationDate { get; set; }

        [JsonIgnore]
        public Distributors Distributors { get; set; }

        public ICollection<DeatilWarehouses> GetDeatilWarehouses { get; set; }
    }
}

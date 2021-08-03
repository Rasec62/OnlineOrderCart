using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineOrderCart.Common.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.Models
{
    public class AddProductViewModel : Products
    {
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }
        public IEnumerable<SelectListItem> ComboProdTypes { get; set; }
        public IEnumerable<SelectListItem> ComboTrademarks { get; set; }
        public IEnumerable<SelectListItem> ComboSimTypes { get; set; }
        public IEnumerable<SelectListItem> ComboActivationForms { get; set; }
        public IEnumerable<SelectListItem> ComboActivationTypes { get; set; }
    }
}

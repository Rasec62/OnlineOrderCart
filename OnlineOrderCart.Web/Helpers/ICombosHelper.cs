using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.Helpers
{
    public interface ICombosHelper
    {
        IEnumerable<SelectListItem> GetComboTrademarks();
        IEnumerable<SelectListItem> GetComboProdcutTypes();
        IEnumerable<SelectListItem> GetComboGenders();
        IEnumerable<SelectListItem> GetComboRoles();
        IEnumerable<SelectListItem> GetComboKams();
        IEnumerable<SelectListItem> GetComboDisRoles();
        IEnumerable<SelectListItem> GetComboAllKams();
        IEnumerable<SelectListItem> GetComboActivationForms();
        IEnumerable<SelectListItem> GetComboActivationTypes();
        IEnumerable<SelectListItem> GetComboSimTypes();
        IEnumerable<SelectListItem> GetComboDistributors();
        IEnumerable<SelectListItem> GetComboProdcuts();
        IEnumerable<SelectListItem> GetComboPurposes();
        IEnumerable<SelectListItem> GetComboWarehouses(long id);
        IEnumerable<SelectListItem> GettoNextComboProducts(long id, int SimTypeId);
        IEnumerable<SelectListItem> GettoNextDisComboProducts(long id, int SimTypeId);
        IEnumerable<SelectListItem> GetOrderStatuses();
        IEnumerable<SelectListItem> GetNextDWComboProducts(long id);
        IEnumerable<SelectListItem> GetComboTypeofPayments();
    }
}

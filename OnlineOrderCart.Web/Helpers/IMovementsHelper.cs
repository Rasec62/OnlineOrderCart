using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineOrderCart.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.Helpers
{
    public interface IMovementsHelper
    {
        IEnumerable<SelectListItem> GetSqlDataKams();
        IEnumerable<SelectListItem> GetSqlComboAllKams();
        IEnumerable<IndexUserDistEntity> GetSqlAllDataDistributors();
    }
}

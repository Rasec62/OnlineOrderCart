using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Common.Responses;
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
        Task<Response<RolAvatarConfirm>> GetRolAvatarConfirmAsync(string UserName);
        List<IndexUserDistEntity> GetSqlforwardingDataDistributors();
        Task<Response<UserActivations>> GetSqlEmailforwardingDataDistributors(long UserId, string UserName, string Email);
        Task<List<TblOptionalEmail>> GetDetailsOptionalEmailAsync(long UserId, string Debtor);
    }
}

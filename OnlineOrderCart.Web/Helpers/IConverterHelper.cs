using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Common.Enums;
using OnlineOrderCart.Common.Responses;
using OnlineOrderCart.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.Helpers
{
    public interface IConverterHelper
    {
        Products ToProductEntity(AddProductViewModel model, bool isNew);
        AddProductViewModel ToProductViewModel(Products model);
        Response<object> CreateSHA256(string Pass);
        Users ToRegisterAsync(AddUserViewModel model, Guid imageId, bool v);
        Task<Users> ToSRegisterAsync(AddUserViewModel model, Guid imageId, bool v);
        public Genders ToGenderStatus(int GenderStatusId);

        #region IMyDistributor
        Task<Users> ToDistributorAsync(AddDistributorViewModel model, Guid imageId, bool v);
        Task<AddDistributorViewModel> ToDistributorViewModel(Users model);
        TokenResponse GetToken(string Username);
        Warehouses ToWarehousetEntity(IndexWarehouseDistViewModel model, bool isNew);
        #endregion

        Task<PrOrderDetailTmps> ToOrdersTmpEntity(AddItemViewModel model, bool isNew);
        Task<PrOrderDetailTmps> ToGenerateaNormalOrdersTmpEntity(GenerateaNormalOrderViewModel model, bool isNew);

    }
}

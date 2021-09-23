using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Common.Responses;
using OnlineOrderCart.Web.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.Helpers
{
    public interface IDistributorHelper
    {
        Task<ObservableCollection<IndexUserDistEntity>> GetAllDistrsRecordsAsync();
        Task<Response<Users>> AddDistributorAsync(AddDistributorViewModel model, Guid imageId);
        Task<Response<AddDistributorViewModel>> GetDistrByEmailAsync(string email);
        Task<Response<AddDistributorViewModel>> GetDistrByIdAsync(long Id);
        Task<List<Warehouses>> GetWarehousesList(AddDistributorViewModel model);
        Task<Response<Warehouses>> GetAddWarehouses(IndexDWarehouseViewModel model);
        Task<Response<object>> PostAddWarehouseOtherProduct(DeatilWarehouses model);
        Task<Response<Distributors>> GetByDistrEmailAsync(string Debtor);
        Task<Response<AddDistributorViewModel>> GetDistrBySentIdAsync(long Id);
    }
}

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Common.Responses;
using OnlineOrderCart.Web.DataBase;
using OnlineOrderCart.Web.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.Helpers
{
    public class DistributorHelper : IDistributorHelper
    {
        private readonly DataContext _dataContext;
        private readonly IConverterHelper _converterHelper;
        private ObservableCollection<IndexUserDistEntity> MyManger;
        public List<Warehouses> _List = new List<Warehouses>();
        public DistributorHelper(DataContext dataContext, IConverterHelper converterHelper)
        {
            _dataContext = dataContext;
            _converterHelper = converterHelper;
            MyManger = new ObservableCollection<IndexUserDistEntity>();
        }
        public async Task<ObservableCollection<IndexUserDistEntity>> GetAllDistrsRecordsAsync()
        {
            try
            {
                var myProducts = await _dataContext
                                     .Users
                                     .Include(r => r.GetDistributorsCollection)
                                     .Include(u => u.GetKamsCollection)
                                     .Include(k => k.GetDistributorsCollection)
                                     .Include(rg => rg.GetRoleGroupUsersCollection)
                                     .Include(r => r.GetRoleGroupUsersCollection.FirstOrDefault().Roles)
                                     .Where(u => u.IsDeleted == 0 && u.IsDistributor == 1).ToListAsync();


                this.MyManger = new ObservableCollection<IndexUserDistEntity>(
                myProducts.Select(p => new IndexUserDistEntity
                {
                    UserId = p.UserId,
                    KamId = p.GetKamsCollection.FirstOrDefault().KamId,
                    Email = p.Email,
                    EmployeeNumber = p.GetKamsCollection.FirstOrDefault().EmployeeNumber,
                    FirstName = p.FirstName,
                    LastName1 = p.LastName1,
                    LastName2 = p.LastName2,
                    GenderId = Convert.ToInt32(p.GenderId),
                    Gender = _converterHelper.ToGenderStatus(Convert.ToInt32(p.GenderId)).ToString(),
                    PicturePath = p.ImageFullPath,
                    Username = p.UserName,
                    RolName = p.GetRoleGroupUsersCollection.FirstOrDefault().Roles.RolName,
                    BusinessName = p.GetDistributorsCollection.FirstOrDefault().BusinessName,
                    DistributorId = p.GetDistributorsCollection.FirstOrDefault().DistributorId,
                })
            .Where(p => p.IsDistributor == 1)
            .OrderBy(p => p.Username)
            .ToList());
                return MyManger;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<Response<Users>> AddDistributorAsync(AddDistributorViewModel model, Guid imageId)
        {
            using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = _dataContext.Database.BeginTransaction())
            {
                try
                {
                    string ND = $"D{model.Debtor}";
                    var _register = await _dataContext.Users
                        .Where(u => u.Email.Equals(model.Email) || u.UserName == ND || u.GetDistributorsCollection.FirstOrDefault().Debtor.ToUpper() == model.Debtor.ToString().ToUpper())
                        .FirstOrDefaultAsync();
                    if (_register != null)
                    {
                        return new Response<Users>
                        {
                            IsSuccess = false,
                            Message = "hay información sobre la aplicación. Registrarse",
                        };
                    }

                    Roles _roles = await _dataContext.Roles.FindAsync(model.RoleId);

                    if (_roles == null)
                    {
                        return new Response<Users>
                        {
                            IsSuccess = false,
                            Message = "No hay información sobre la aplicación. Registro de roles",
                        };
                    }
                    Users _users = await _converterHelper.ToDistributorAsync(model, imageId, true);

                    _dataContext.Users.Add(_users);
                    await _dataContext.SaveChangesAsync();

                    Distributors _Dist = new Distributors
                    {
                        UserId = _users.UserId,
                        BusinessName = model.BusinessName,
                        Debtor = model.Debtor.ToString(),
                        KamId = model.KamId,
                        MD = model.MD,
                        IsDeleted = 100,
                        RegistrationDate = DateTime.Now.ToUniversalTime(),
                    };

                    _dataContext.Distributors.Add(_Dist);

                    RoleGroups UsersRols = await _dataContext
                    .RoleGroups
                    .Where(r => r.RolId == model.RoleId && r.UserId == _users.UserId)
                    .FirstOrDefaultAsync();

                    if (UsersRols != null)
                    {
                        return new Response<Users>
                        {
                            IsSuccess = false,
                            Message = "No hay información sobre la aplicación.",
                        };
                    }

                    RoleGroups usersRole = new RoleGroups
                    {
                        IsDeleted = 0,
                        RegistrationDate = DateTime.Now.ToUniversalTime(),
                        RolId = model.RoleId,
                        UserId = _users.UserId,
                    };
                    _dataContext.RoleGroups.Add(usersRole);
                    await _dataContext.SaveChangesAsync();

                    transaction.Commit();
                    return new Response<Users>
                    {
                        IsSuccess = true,
                        Message = "The information has already been registered in the system.",
                        Result = _users,
                    };
                }
                catch (Exception exception)
                {
                    transaction.Rollback();
                    return new Response<Users>
                    {
                        IsSuccess = false,
                        Message = exception.InnerException.Message,
                    };
                }
            }
        }
        public async Task<Response<AddDistributorViewModel>> GetDistrByEmailAsync(string email)
        {
            try
            {
                var _Distri = await (from rg in _dataContext.RoleGroups
                              join r in _dataContext.Roles on rg.RolId equals r.RolId
                              join u in _dataContext.Users on rg.UserId equals u.UserId
                              join d in _dataContext.Distributors on u.UserId equals d.UserId
                              join k in _dataContext.Kams on d.KamId equals k.KamId
                              where(u.UserName == email && u.IsDeleted == 0  && d.IsDeleted == 0 && u.IsDistributor ==1)
                              select new AddDistributorViewModel {
                                   BusinessName = d.BusinessName,
                                   Debtor = Convert.ToInt32(d.Debtor.ToString()),
                                   DistributorId = d.DistributorId,
                                   MD = d.MD,
                                   UserId = u.UserId,
                                   Username = u.UserName,
                                   GenderId = Convert.ToInt32(u.GenderId),
                                   Email = u.Email,
                                   RoleId = r.RolId,
                                   KamId = k.KamId,
                                   KamName = $"{k.Users.FirstName}{" "}{k.Users.LastName1}{" "}{k.Users.LastName2}",
                                   ImageId = u.ImageId,
                                   PicturePath = u.ImageFullPath,

                              }).FirstOrDefaultAsync();
                if (_Distri == null){
                    return new Response<AddDistributorViewModel>
                    {
                        IsSuccess = false,
                        Message = "No data Distributor",
                    };
                }

                return new Response<AddDistributorViewModel>
                {
                    IsSuccess = true,
                    Message = "Success",
                    Result = _Distri,
                };
            }
            catch (Exception exception)
            {
                return new Response<AddDistributorViewModel>
                {
                    IsSuccess = false,
                    Message = exception.InnerException.Message,
                };
            }
        }
        public async Task<Response<AddDistributorViewModel>> GetDistrByIdAsync(long Id)
        {
            try{
                var _Distri = await (from rg in _dataContext.RoleGroups
                                     join r in _dataContext.Roles on rg.RolId equals r.RolId
                                     join u in _dataContext.Users on rg.UserId equals u.UserId
                                     join d in _dataContext.Distributors on u.UserId equals d.UserId
                                     join k in _dataContext.Kams on d.KamId equals k.KamId
                                     where (d.DistributorId == Id && u.IsDeleted == 0 && d.IsDeleted == 0 && u.IsDistributor == 1)
                                     select new AddDistributorViewModel
                                     {
                                         BusinessName = d.BusinessName,
                                         Debtor = Convert.ToInt32(d.Debtor.ToString()),
                                         DistributorId = d.DistributorId,
                                         MD = d.MD,
                                         UserId = u.UserId,
                                         Username = u.UserName,
                                         GenderId = Convert.ToInt32(u.GenderId),
                                         Email = u.Email,
                                         RoleId = r.RolId,
                                         KamId = k.KamId,
                                         ImageId = u.ImageId,
                                         PicturePath = u.ImageFullPath,
                                         PictureFullPath = u.PictureFullPath
                                     }).FirstOrDefaultAsync();
                if (_Distri == null)
                {
                    return new Response<AddDistributorViewModel>
                    {
                        IsSuccess = false,
                        Message = "No data Distributor",
                    };
                }

                return new Response<AddDistributorViewModel>
                {
                    IsSuccess = true,
                    Message = "Success",
                    Result = _Distri,
                };
            }
            catch (Exception exception)
            {
                return new Response<AddDistributorViewModel>
                {
                    IsSuccess = false,
                    Message = exception.InnerException.Message,
                };
            }
        }
        public async Task<List<Warehouses>> GetWarehousesList(AddDistributorViewModel model)
        {
            try{
                _List = await (from w in _dataContext.Warehouses
                               //join dw in _dataContext.DeatilWarehouses on w.StoreId equals dw.StoreId
                               join d in _dataContext.Distributors on w.DistributorId equals d.DistributorId
                               join u in _dataContext.Users on d.UserId equals u.UserId
                               //join pu in _dataContext.Purposes on dw.PurposeId equals pu.PurposeId
                               //join p in _dataContext.Products on dw.ProductId equals p.ProductId
                               where (u.UserName == model.Username && d.UserId == model.UserId && u.IsDeleted == 0)
                               select new Warehouses
                               {
                                   DelegationMunicipality = w.DelegationMunicipality,
                                   DistributorId = w.DistributorId,
                                   SapDescription = w.SapDescription,
                                   PostalCode = w.PostalCode,
                                   SapClient = w.SapClient,
                                   RegistrationDate = w.RegistrationDate,
                                   IsDeleted = w.IsDeleted,
                                   ShippingBranchName = w.ShippingBranchName,
                                   ShippingBranchNo = w.ShippingBranchNo,
                                   State = w.State,
                                   StoreId = w.StoreId,
                                   StreetNumber = w.StreetNumber,
                                   Suburd = w.Suburd,
                                   Distributors = w.Distributors,
                                   Warehousepvs = w.Warehousepvs,
                               }).ToListAsync();

                if (_List != null){
                    return _List;
                }
                else{
                    return null;
                }
            }
            catch (Exception ex){
                throw new Exception($"{"No show to Warehouse List"} { ex.Message}");
            }
        }
        public async Task<Response<Warehouses>> GetAddWarehouses(IndexDWarehouseViewModel model)
        {
            using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = _dataContext.Database.BeginTransaction()) {
                try
                {
                    var _count = await _dataContext
                        .DeatilWarehouses
                        .Include(w => w.Warehouses)
                        .Where(w => w.ProductId.Equals(model.ProductId) && w.Warehouses.DistributorId == model.DistributorId)
                        .ToListAsync();

                    if (_count.Count > 0) {
                        return new Response<Warehouses>
                        {
                            IsSuccess = false,
                            Message = "Products Duplic......",
                        };
                    }

                    var warehouse = new Warehouses {
                        DelegationMunicipality = model.DelegationMunicipality,
                        PostalCode = model.PostalCode,
                        DistributorId = model.DistributorId,
                        SapClient = model.SapClient,
                        SapDescription = model.SapDescription,
                        ShippingBranchName = model.ShippingBranchName,
                        ShippingBranchNo = model.ShippingBranchNo,
                        StreetNumber = model.StreetNumber,
                        State = model.State,
                        Suburd = model.Suburd,
                        Warehousepvs = model.Warehousepvs,
                        SimTypeId = model.SimTypeId,
                        IsDeleted = 0,
                        RegistrationDate = DateTime.Now.ToUniversalTime(),
                    };

                    _dataContext.Warehouses.Add(warehouse);
                    await _dataContext.SaveChangesAsync();

                    var _WDResult = new DeatilWarehouses {
                        ProductId = model.ProductId,
                        PurposeId = model.PurposeId,
                        StoreId = warehouse.StoreId,
                        IsDeleted = 0,
                        RegistrationDate = DateTime.Now.ToUniversalTime(),
                    };
                    _dataContext.DeatilWarehouses.Add(_WDResult);
                    await _dataContext.SaveChangesAsync();

                    transaction.Commit();
                }
                catch (SqlException exception) {
                    if (exception.Number == 2601){ // Cannot insert duplicate key row in object error
                        transaction.Rollback();
                        return new Response<Warehouses>
                        {
                            IsSuccess = false,
                            Message = $"{exception.Message}{"-  duplicate key"}",
                        };
                    }
                }
                catch (Exception exception){
                    transaction.Rollback();
                    return new Response<Warehouses>
                    {
                        IsSuccess = false,
                        Message = exception.InnerException.Message,
                    };
                }
            }
            return new Response<Warehouses>{
                IsSuccess = true,
                Message = "ok",
                Result = model
            };
        }
        public async Task<Response<object>> PostAddWarehouseOtherProduct(DeatilWarehouses model) {
            try
            {
                _dataContext.DeatilWarehouses.Add(model);
                await _dataContext.SaveChangesAsync();
                return new Response<object>
                {
                    IsSuccess = true,
                };
            }
            catch (Exception exception)
            {
                return new Response<object>
                {
                    IsSuccess = false,
                    Message = exception.InnerException.Message,
                };
            }
        }
        public async Task<Response<Distributors>> GetByDistrEmailAsync(string Debtor)
        {
            try{
                var Distr = await _dataContext
                    .Distributors
                    .Where(d => d.Debtor == Debtor)
                    .FirstOrDefaultAsync();
                return new Response<Distributors>
                {
                    IsSuccess = true,
                    Message = "ok",
                    Result = Distr,
                };
            }
            catch (Exception ex){
                return new Response<Distributors> {
                        IsSuccess = false,
                       Message = ex.Message,

                };
            }
        }
        public async Task<Response<AddDistributorViewModel>> GetDistrBySentIdAsync(long Id)
        {
            try
            {
                var _Distri = await (from rg in _dataContext.RoleGroups
                                     join r in _dataContext.Roles on rg.RolId equals r.RolId
                                     join u in _dataContext.Users on rg.UserId equals u.UserId
                                     join d in _dataContext.Distributors on u.UserId equals d.UserId
                                     join k in _dataContext.Kams on d.KamId equals k.KamId
                                     where (d.DistributorId == Id && u.IsDeleted == 100 && d.IsDeleted == 100 && u.IsDistributor == 1)
                                     select new AddDistributorViewModel
                                     {
                                         BusinessName = d.BusinessName,
                                         Debtor = Convert.ToInt32(d.Debtor.ToString()),
                                         DistributorId = d.DistributorId,
                                         MD = d.MD,
                                         UserId = u.UserId,
                                         Username = u.UserName,
                                         GenderId = Convert.ToInt32(u.GenderId),
                                         Email = u.Email,
                                         RoleId = r.RolId,
                                         KamId = k.KamId,
                                         ImageId = u.ImageId,
                                         PicturePath = u.ImageFullPath,
                                         PictureFullPath = u.PictureFullPath
                                     }).FirstOrDefaultAsync();
                if (_Distri == null)
                {
                    return new Response<AddDistributorViewModel>
                    {
                        IsSuccess = false,
                        Message = "No data Distributor",
                    };
                }

                return new Response<AddDistributorViewModel>
                {
                    IsSuccess = true,
                    Message = "Success",
                    Result = _Distri,
                };
            }
            catch (Exception exception)
            {
                return new Response<AddDistributorViewModel>
                {
                    IsSuccess = false,
                    Message = exception.InnerException.Message,
                };
            }
        }
    }
}

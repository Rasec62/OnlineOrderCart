using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Common.Responses;
using OnlineOrderCart.Web.DataBase;
using OnlineOrderCart.Web.DataBase.Repositories;
using OnlineOrderCart.Web.Helpers;
using OnlineOrderCart.Web.Models;
using OnlineOrderCart.Web.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Vereyon.Web;

namespace OnlineOrderCart.Web.Controllers
{
    public class DistributorsController : Controller
    {
        private readonly IFlashMessage _flashMessage;
        private readonly IConfiguration _configuration;
        private readonly IBlobHelper _blobHelper;
        private readonly DataContext _dataContext;
        private readonly IDistributorHelper _distributorHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IMovementsHelper _movementsHelper;
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IDapper _dapper;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IImageHelper _imageHelper;

        public DistributorsController(IFlashMessage flashMessage,
            IConfiguration configuration, IBlobHelper blobHelper, 
            DataContext dataContext, IDistributorHelper distributorHelper
            ,ICombosHelper combosHelper, IMovementsHelper movementsHelper
            , IUserHelper userHelper, IConverterHelper converterHelper, IMailHelper mailHelper
            , IDapper dapper, IWarehouseRepository warehouseRepository, IImageHelper ImageHelper)
        {
           _flashMessage = flashMessage;
           _configuration = configuration;
           _blobHelper = blobHelper;
           _dataContext = dataContext;
           _distributorHelper = distributorHelper;
           _combosHelper = combosHelper;
           _movementsHelper = movementsHelper;
           _userHelper = userHelper;
           _converterHelper = converterHelper;
           _mailHelper = mailHelper;
           _dapper = dapper;
           _warehouseRepository = warehouseRepository;
           _imageHelper = ImageHelper;
        }

        public async Task<IActionResult> IndexDistributor()
        {
            if (User.Identity.Name == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            var _users = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            if (!_users.IsSuccess || _users.Result == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            return View( _movementsHelper.GetSqlAllDataDistributors());
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet]
        [Authorize(Roles = "PowerfulUser,KamAdmin,KamAdCoordinator")]
        public IActionResult CreateDistributor()
        {
            AddDistributorViewModel model = new AddDistributorViewModel{
               Password = _configuration["SecretP:SecretPassword"],
               PasswordConfirm = _configuration["SecretP:SecretPassword"],
               ComboGenders = _combosHelper.GetComboGenders(),
               ComboDisRoles = _combosHelper.GetComboDisRoles(),
               ComboKams = _movementsHelper.GetSqlComboAllKams(),
               GenderId = 3,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDistributor(AddDistributorViewModel model)
        {
            if (ModelState.IsValid) {
                string path = model.PicturePath;
                if (User.Identity.Name == null)
                {
                    return new NotFoundViewResult("_ResourceNotFound");
                }
                var _users = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
                if (!_users.IsSuccess || _users.Result == null)
                {
                    return new NotFoundViewResult("_ResourceNotFound");
                }
                Guid imageId = Guid.Empty;
                if (model.ImageFile != null)
                {
                    //imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                    path = await _imageHelper.UploadImageAsync(model.ImageFile, "users");
                }
                model.PicturePath = path; 
                Response<Users> user = await _distributorHelper.AddDistributorAsync(model, imageId);
               
                if (user == null)
                {
                    _flashMessage.Danger(user.Message, "This email is already used.");
                    ModelState.AddModelError(string.Empty, "This email is already used.");
                    model.ComboGenders = _combosHelper.GetComboGenders();
                    model.ComboDisRoles = _combosHelper.GetComboDisRoles();
                    model.ComboKams = _movementsHelper.GetSqlComboAllKams();
                    return View(model);
                }

                TokenResponse myToken = _converterHelper.GetToken(user.Result.Email);

                Guid activationCode = Guid.NewGuid();
                var userActivations = new UserActivations
                {
                    ActivationCode = activationCode,
                    UserId = user.Result.UserId,
                    UserName = user.Result.UserName,
                    EventAction = "ConfirmEmail - Distributor",
                    JwtId = myToken.Token,
                    CreationDate = DateTime.UtcNow.ToUniversalTime(),
                    ExpiryDate = myToken.Expiration,
                    IsDeleted = 0,
                    RegistrationDate = DateTime.Now.ToUniversalTime(),
                };

                _dataContext.UserActivations.Add(userActivations);
                await _dataContext.SaveChangesAsync();
                string Password = _configuration["SecretP:SecretPassword"];
                string tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userid = user.Result.UserId,
                    username = user.Result.UserName,
                    Jwt = activationCode,
                    token = myToken.Token,
                }, protocol: HttpContext.Request.Scheme);

                Response<object> response = _mailHelper.SendMail(model.Email, $"Email confirmation. this is your password :{Password}", $"<h1>Email Confirmation</h1>" +
                    $"To allow the Distributor, " +
                    $"plase click in this link:<p><a href = \"{tokenLink}\">Confirm Email . this is your password :{Password}</a></p>");
                if (response.IsSuccess)
                {
                    ViewBag.Message = "The instructions to allow your user has been sent to email.";
                    _flashMessage.Confirmation("The instructions to allow your user has been sent to email.");
                    return RedirectToAction("IndexDistributor", "Distributors");
                }

                ModelState.AddModelError(string.Empty, response.Message);
                _flashMessage.Danger(response.Message, "This Confirm Email is already used.");


            }
            model.ComboGenders = _combosHelper.GetComboGenders();
            model.ComboDisRoles = _combosHelper.GetComboDisRoles();
            model.ComboKams = _movementsHelper.GetSqlComboAllKams();
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "PowerfulUser,KamAdmin,KamAdCoordinator")]
        public async Task<IActionResult> DeleteDistributor(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            try
            {
                var dist = await DistributorsExists(id.Value);

                if (dist == null)
                {
                    return new NotFoundViewResult("_ResourceNotFound");
                }

                dist.IsDeleted = 1;
                _dataContext.Distributors.Update(dist);
                await _dataContext.SaveChangesAsync();
                var _user = await _dataContext.Users.FindAsync(dist.UserId);
                if (_user == null)
                {
                    return new NotFoundViewResult("_ResourceNotFound");
                }
                _user.IsDeleted = 1;
                await _dataContext.SaveChangesAsync();
                _flashMessage.Confirmation("The Trademars was deleted.");
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"The Products can't be deleted because it has related records. {ex.Message}");
            }
            return RedirectToAction(nameof(Index));
        }
        private async Task<Distributors> DistributorsExists(int id)
        {
            var _dist = await _dataContext.Distributors.FindAsync(id);
            return _dist;
        }

        [HttpGet]
        [Authorize(Roles = "Distributor")]
        public async Task<IActionResult> DistChangeUser(){

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (User.Identity.Name == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            var model = await _distributorHelper.GetDistrByEmailAsync(User.Identity.Name);
            if (model.Result == null || !model.IsSuccess)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            model.Result.ComboGenders = _combosHelper.GetComboGenders();
            model.Result.ComboDisRoles = _combosHelper.GetComboDisRoles();
            model.Result.ComboKams = _movementsHelper.GetSqlComboAllKams();
            return View(model.Result);
        }

        [HttpGet]
        [Authorize(Roles = "PowerfulUser,KamAdmin,KamAdCoordinator")]
        public async Task<IActionResult> EditDistributor(int? id)
        {
            if (User.Identity.Name == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            var _users = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            if (!_users.IsSuccess || _users.Result == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            long _Id = Convert.ToInt64(id);
            var _Dis = await _distributorHelper.GetDistrByIdAsync(_Id);
            if (_Dis == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            
            var model = new AddDistributorViewModel
            {
                UserId = _Dis.Result.UserId,
                KamId = _Dis.Result.KamId,
                FirstName = _Dis.Result.FirstName,
                LastName1 = _Dis.Result.LastName1,
                LastName2 = _Dis.Result.LastName2,
                GenderId = _Dis.Result.GenderId,
                Email = _Dis.Result.Email,
                Username = _Dis.Result.Username,
                RoleId = _Dis.Result.RoleId,
                PicturePath = _Dis.Result.PicturePath,
                BusinessName = _Dis.Result.BusinessName,
                Debtor = _Dis.Result.Debtor,
                ImageId = _Dis.Result.ImageId,
                PictureFullPath = _Dis.Result.PictureFullPath,
                MD = _Dis.Result.MD,
                Password = "D*12345678",
                PasswordConfirm = "D*12345678",
            };

            model.ComboGenders = _combosHelper.GetComboGenders();
            model.ComboDisRoles = _combosHelper.GetComboDisRoles();
            model.ComboKams = _movementsHelper.GetSqlComboAllKams();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDistributor(AddDistributorViewModel model) {
            if (ModelState.IsValid) {
                string path = model.PicturePath;

                Guid imageId = model.ImageId;

                if (model.ImageFile != null)
                {
                    //imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                    path = await _imageHelper.UploadImageAsync(model.ImageFile, "users");
                }
                using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = _dataContext.Database.BeginTransaction())
                {
                    try
                    {

                        Users _users = await _dataContext
                           .Users
                           .Where(u => u.UserId == model.UserId && u.UserName == model.Username)
                           .FirstOrDefaultAsync();

                        _users.FirstName = model.FirstName ?? _users.FirstName;
                        _users.LastName1 = model.LastName1 ?? _users.LastName1;
                        _users.LastName2 = model.LastName2 ?? _users.LastName2;
                        _users.GenderId = model.GenderId.ToString() ?? _users.GenderId;
                        _users.Email = model.Email ?? _users.Email;
                        _users.UserName = model.Username ?? _users.UserName;
                        _users.ImageId = imageId;
                        _users.PicturePath = path;

                        _dataContext.Users.Update(_users);

                        Distributors _Dist = await _dataContext
                            .Distributors
                            .Where(k => k.KamId == model.KamId && k.UserId == _users.UserId).FirstOrDefaultAsync();

                        if (_Dist == null)
                        {
                            return new NotFoundViewResult("_ResourceNotFound");
                        }

                        _Dist.BusinessName = model.BusinessName ?? _Dist.BusinessName;
                        _Dist.KamId = model.KamId == 0 ? model.KamId : _Dist.KamId;
                        _Dist.Debtor = model.Debtor.ToString()??_Dist.Debtor;
                        _Dist.MD = model.MD ?? _Dist.MD;

                        _dataContext.Distributors.Update(_Dist);
                        await _dataContext.SaveChangesAsync();
                        transaction.Commit();
                        return RedirectToAction("IndexDistributor", "Distributors");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        model.ComboGenders = _combosHelper.GetComboGenders();
                        model.ComboDisRoles = _combosHelper.GetComboDisRoles();
                        model.ComboKams = _movementsHelper.GetSqlComboAllKams();
                        _flashMessage.Danger(ex.Message, "This email is already used.");
                        return View(model);
                    }
                }
            }
            model.ComboGenders = _combosHelper.GetComboGenders();
            model.ComboDisRoles = _combosHelper.GetComboDisRoles();
            model.ComboKams = _movementsHelper.GetSqlComboAllKams();
            return View(model);
        }
        [HttpGet]
        [Authorize(Roles = "PowerfulUser,KamAdmin,KamAdCoordinator")]
        public async Task<IActionResult> DetailDistributor(int? id)
        {
            if (User.Identity.Name == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            var _users = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            if (!_users.IsSuccess || _users.Result == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            long _Id = Convert.ToInt64(id);
            var model = await _distributorHelper.GetDistrByIdAsync(_Id);
            if (model.Result ==  null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            return View(model.Result);
        }

        [HttpGet]
        [Authorize(Roles = "PowerfulUser,KamAdmin,KamAdCoordinator")]
        public async Task<IActionResult> IndexWarehouseDist(int? id)
        {
            if (User.Identity.Name == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            var _users = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            if (!_users.IsSuccess || _users.Result == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            long _Id = Convert.ToInt64(id);
            var _Dis = await _distributorHelper.GetDistrByIdAsync(_Id);
            if (_Dis == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            var model = new IndexWarehouseDistViewModel
            {
                UserId = _Dis.Result.UserId,
                KamId = _Dis.Result.KamId,
                DistributorId = _Dis.Result.DistributorId,
                UserName = _Dis.Result.Username,
                BusinessName = _Dis.Result.BusinessName,
                Debtor = _Dis.Result.Debtor.ToString(),
            };
            model.DetailWarehousess = await _distributorHelper.GetWarehousesList(_Dis.Result);
            model.ComboDistributors = _combosHelper.GetComboDistributors().Where(d => d.Value == _Dis.Result.DistributorId.ToString());
            //model.ComboProducts = _combosHelper.GetComboProdcuts();
            model.ComboPurposes = _combosHelper.GetComboPurposes();
            model.ComboSimTypes = _combosHelper.GetComboSimTypes();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexWarehouseDist(IndexWarehouseDistViewModel model)
        {
            var _Dis = await _distributorHelper.GetDistrByIdAsync(model.DistributorId);
            if (ModelState.IsValid)
            {
                if (User.Identity.Name == null)
                {
                    return new NotFoundViewResult("_ResourceNotFound");
                }
                var _users = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
                if (!_users.IsSuccess || _users.Result == null)
                {
                    return new NotFoundViewResult("_ResourceNotFound");
                }
                var IndexWarehouse = _converterHelper.ToWarehousetEntity(model, true);

                var Dmodel = new IndexDWarehouseViewModel
                {
                    DelegationMunicipality = IndexWarehouse.DelegationMunicipality,
                    PostalCode = IndexWarehouse.PostalCode,
                    ProductId = model.ProductId,
                    PurposeId = model.PurposeId,
                    DistributorId = IndexWarehouse.DistributorId,
                    SapClient = IndexWarehouse.SapClient,
                    SapDescription = IndexWarehouse.SapDescription,
                    ShippingBranchName = IndexWarehouse.ShippingBranchName,
                    ShippingBranchNo = IndexWarehouse.ShippingBranchNo,
                    StreetNumber = IndexWarehouse.StreetNumber,
                    State = IndexWarehouse.State,
                    Suburd = IndexWarehouse.Suburd,
                    Warehousepvs = IndexWarehouse.Warehousepvs,
                    StoreId = IndexWarehouse.StoreId,
                    SimTypeId = IndexWarehouse.SimTypeId,
                };

                var _Result = await _distributorHelper.GetAddWarehouses(Dmodel);
                if (!_Result.IsSuccess)
                {
                    _flashMessage.Danger(_Result.Message, "This not data.");
                    model.DetailWarehousess = await _distributorHelper.GetWarehousesList(_Dis.Result);
                    model.ComboDistributors = _combosHelper.GetComboDistributors().Where(d => d.Value == _Dis.Result.DistributorId.ToString());
                    model.ComboProducts = _combosHelper.GetComboProdcuts();
                    model.ComboPurposes = _combosHelper.GetComboPurposes();
                    return View(model);
                }
                return RedirectToAction("IndexWarehouseDist", "Distributors", new { id = model.DistributorId });
            }
            model.DetailWarehousess = await _distributorHelper.GetWarehousesList(_Dis.Result);
            model.ComboDistributors = _combosHelper.GetComboDistributors().Where(d => d.Value == _Dis.Result.DistributorId.ToString());
            //model.ComboProducts = _combosHelper.GetComboProdcuts();
            model.ComboPurposes = _combosHelper.GetComboPurposes();
            return View(model);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> IndexWarehouseDist(IndexWarehouseDistViewModel model)
        //{
        //    var _Dis = await _distributorHelper.GetDistrByIdAsync(model.DistributorId);
        //    if (ModelState.IsValid)
        //    {
        //        if (User.Identity.Name == null)
        //        {
        //            return new NotFoundViewResult("_ResourceNotFound");
        //        }
        //        var _users = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
        //        if (!_users.IsSuccess || _users.Result == null)
        //        {
        //            return new NotFoundViewResult("_ResourceNotFound");
        //        }

        //        var dbparams = new DynamicParameters();

        //        int Result = 0;

        //        dbparams.Add("DistributorId", model.DistributorId, DbType.Int64, ParameterDirection.Output);
        //        dbparams.Add("ProductId", model.ProductId, DbType.Int32);
        //        dbparams.Add("PurposeId", model.ProductId, DbType.Int32);
        //        dbparams.Add("DelegationMunicipality", model.DelegationMunicipality, DbType.String);
        //        dbparams.Add("PostalCode", model.PostalCode, DbType.String);
        //        dbparams.Add("ShippingBranchNo", model.ShippingBranchNo, DbType.String);
        //        dbparams.Add("ShippingBranchName", model.ShippingBranchName, DbType.String);
        //        dbparams.Add("SapClient", model.SapClient, DbType.String);
        //        dbparams.Add("SapDescription", model.SapDescription, DbType.String);
        //        dbparams.Add("Warehousepvs", model.Warehousepvs, DbType.String);
        //        dbparams.Add("Observations", model.Observations, DbType.String);
        //        dbparams.Add("State", model.State, DbType.String);
        //        dbparams.Add("StreetNumber", model.StreetNumber, DbType.String);
        //        dbparams.Add("Suburd", model.Suburd, DbType.String);



        //        Result = dbparams.Get<int>("@Result");

        //        //var result = await Task.FromResult(_dapper.Insert<int>("sp_AddWarehouses"
        //        //    , dbparams,
        //        //    commandType: CommandType.StoredProcedure));

        //        _dapper.Insert<Warehouses>("sp_AddWarehouses", dbparams,commandType: CommandType.StoredProcedure);

        //        return RedirectToAction("IndexWarehouseDist", "Distributors", new { id = model.DistributorId });
        //    }
        //    model.DetailWarehousess = await _distributorHelper.GetWarehousesList(_Dis.Result);
        //    model.ComboDistributors = _combosHelper.GetComboDistributors().Where(d => d.Value == _Dis.Result.DistributorId.ToString());
        //    model.ComboProducts = _combosHelper.GettoNextComboProducts(_Dis.Result.DistributorId);
        //    model.ComboPurposes = _combosHelper.GetComboPurposes();
        //    return View(model);
        //}

        [HttpGet]
        [Authorize(Roles = "PowerfulUser,KamAdmin,KamAdCoordinator")]
        public async Task<IActionResult> DistributorWareDelete(int? id) {
            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            long _id = Convert.ToInt64(id);
            var _ware = await _dataContext.Warehouses.FindAsync(_id);
            if (_ware == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            try
            {
                DateTime saveUtcNow = DateTime.UtcNow;
                _ware.IsDeleted = 1;
                _ware.RegistrationDate = saveUtcNow;
                _dataContext.Update(_ware);
               await _dataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"The Products can't be deleted because it has related records. {ex.Message}");
            }
            return RedirectToAction("IndexWarehouseDist", "Distributors", new { id = _ware.DistributorId });
        }


        [HttpGet]
        [Authorize(Roles = "PowerfulUser,KamAdmin,KamAdCoordinator")]
        public async Task<IActionResult> OtherProduct(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

      
            var _Store = await _dataContext.DeatilWarehouses
                .Include(w => w.Warehouses)
                .ThenInclude(w => w.Distributors)
                .Where(dw => dw.Warehouses.StoreId == id).FirstOrDefaultAsync();

            if (_Store == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            var model = new OtherProductViewModel
            {
                BusinessName = _Store.Warehouses.Distributors.BusinessName,
                Debtor = _Store.Warehouses.Distributors.Debtor,
                ShippingBranchName = _Store.Warehouses.ShippingBranchName,
                StoreId = _Store.StoreId,
                DeatilStoreId = _Store.DeatilStoreId,
                SimTypeId = _Store.Warehouses.SimTypeId,
                DistributorId = _Store.Warehouses.DistributorId,
                ComboProducts = _combosHelper.GettoNextDisComboProducts(Convert.ToInt64(id), _Store.Warehouses.SimTypeId),
                ComboPurposes = _combosHelper.GetComboPurposes()
            };


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OtherProduct(OtherProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.Name == null)
                {
                    return new NotFoundViewResult("_ResourceNotFound");
                }

                var _model = new DeatilWarehouses {
                 StoreId = model.StoreId,
                 PurposeId = model.PurposeId,
                 ProductId = model.ProductId,
                };

                var _R = await _distributorHelper.PostAddWarehouseOtherProduct(_model);

                if (!_R.IsSuccess){
                    _flashMessage.Danger(_R.Message, "This not data.");
                    model.ComboProducts = _combosHelper.GettoNextDisComboProducts(Convert.ToInt64(model.StoreId),model.SimTypeId);
                    model.ComboPurposes = _combosHelper.GetComboPurposes();
                    return View(model);
                }

                return RedirectToAction("IndexWarehouseDist", "Distributors", new { id = model.DistributorId });
            }
            model.ComboProducts = _combosHelper.GettoNextDisComboProducts(Convert.ToInt64(model.StoreId), model.SimTypeId);
            model.ComboPurposes = _combosHelper.GetComboPurposes();
            return View(model);
        }


        [HttpGet]
        public IActionResult OtherProducts(string storeId)
        {
            if (storeId == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            var _storeid = Convert.ToInt64(storeId);
            var _Store = _dataContext.DeatilWarehouses
                .Include(w => w.Warehouses)
                .ThenInclude(w => w.Distributors)
                .Where(dw => dw.Warehouses.StoreId == _storeid).FirstOrDefault();

            if (_Store == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            var model = new OtherProductViewModel {
               BusinessName = _Store.Warehouses.Distributors.BusinessName,
               Debtor = _Store.Warehouses.Distributors.Debtor,
               ShippingBranchName = _Store.Warehouses.ShippingBranchName,
               StoreId =_Store.StoreId,
               DeatilStoreId = _Store.DeatilStoreId,
               DistributorId = _Store.Warehouses.DistributorId,
               ComboProducts =_combosHelper.GettoNextComboProducts(_Store.Warehouses.Distributors.DistributorId, _Store.Warehouses.SimTypeId)
            };


            return PartialView("_OtherProducts", model);
        }


        [HttpGet]
        [Authorize(Roles = "PowerfulUser,KamAdmin,KamAdCoordinator")]
        public async Task<IActionResult> GuardiansofDetailWarehouses(int? id)
        {
            if (User.Identity.Name == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            var _users = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            if (!_users.IsSuccess || _users.Result == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            long _Id = Convert.ToInt64(id);
              var warehouse = await _dataContext.Warehouses
                .Include(d => d.Distributors)
                .Where(w=> w.StoreId == _Id).FirstOrDefaultAsync();

            if (warehouse == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            var model = new DeatilWarehouseViewModel {
                StoreId = warehouse.StoreId,
                DelegationMunicipality = warehouse.DelegationMunicipality,
                DistributorId = warehouse.DistributorId,
                Observations = warehouse.Observations,
                PostalCode = warehouse.PostalCode,
                SapClient = warehouse.SapClient,
                SapDescription = warehouse.SapDescription,
                ShippingBranchName = warehouse.ShippingBranchName,
                ShippingBranchNo = warehouse.ShippingBranchNo,
                State = warehouse.State,
                StreetNumber = warehouse.StreetNumber,
                Suburd = warehouse.Suburd,
                Distributors = warehouse.Distributors,
                Warehousepvs = warehouse.Warehousepvs,
            };

            model.DetailWarehouses = await _dataContext.DeatilWarehouses
                .Include(p => p.Products)
                .Include(po => po.Purposes)
                .Where(x => x.StoreId == warehouse.StoreId).ToListAsync();

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "PowerfulUser,KamAdmin,KamAdCoordinator")]
        public async Task<IActionResult> EditofDetailWarehouses(int? id) {
            
            if (User.Identity.Name == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            var _users = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            if (!_users.IsSuccess || _users.Result == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            var model = await _warehouseRepository.GetOnlyWarehouseAsync(id.Value);

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditofDetailWarehouses(Warehouses model)
        {
            if (ModelState.IsValid) {
                if (User.Identity.Name == null)
                {
                    return new NotFoundViewResult("_ResourceNotFound");
                }
                try
                {
                    Warehouses _Warehouse = await _dataContext
                           .Warehouses
                           .Where(w => w.DistributorId == model.DistributorId && w.StoreId == model.StoreId)
                           .FirstOrDefaultAsync();
                    if (_Warehouse == null)
                    {
                        return new NotFoundViewResult("_ResourceNotFound");
                    }

                    _Warehouse.Suburd = model.Suburd ?? _Warehouse.Suburd;
                    _Warehouse.ShippingBranchNo = model.ShippingBranchNo ?? _Warehouse.ShippingBranchNo;
                    _Warehouse.ShippingBranchName = model.ShippingBranchName ?? _Warehouse.ShippingBranchName;
                    _Warehouse.SapDescription = model.SapDescription ?? _Warehouse.SapDescription;
                    _Warehouse.SapClient = model.SapClient ?? _Warehouse.SapClient;
                    _Warehouse.PostalCode = model.PostalCode ?? _Warehouse.PostalCode;
                    _Warehouse.Observations = model.Observations ?? _Warehouse.Observations;
                    _Warehouse.State = model.State ?? _Warehouse.State;
                    _Warehouse.StreetNumber = model.StreetNumber ?? _Warehouse.StreetNumber;
                    _Warehouse.Warehousepvs = model.Warehousepvs ?? _Warehouse.Warehousepvs;
                    _Warehouse.DelegationMunicipality = model.DelegationMunicipality ?? _Warehouse.DelegationMunicipality;

                    _dataContext.Warehouses.Update(_Warehouse);
                    await _dataContext.SaveChangesAsync();

                    return RedirectToAction("IndexWarehouseDist", "Distributors", new { id = model.DistributorId });

                }
                catch (Exception ex)
                {
                    _flashMessage.Danger(ex.Message);
                }
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "PowerfulUser,KamAdmin,KamAdCoordinator")]
        public async Task<IActionResult> DetailWarehouseDelete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            var DetailWarehouse = await DeatilWarehousesExists(id.Value);
            try{
                if (DetailWarehouse == null)
                {
                    return new NotFoundViewResult("_ResourceNotFound");
                }

                DeatilWarehouses dwmodel = await _dataContext
                    .DeatilWarehouses
                    .FindAsync(DetailWarehouse.DeatilStoreId);

                dwmodel.IsDeleted = 1;
                dwmodel.RegistrationDate = DateTime.Now.ToUniversalTime();
                _dataContext.DeatilWarehouses.Update(dwmodel);
                await _dataContext.SaveChangesAsync();
                
                _flashMessage.Confirmation("The DeatilWarehouses was deleted.");
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"The Products can't be deleted because it has related records. {ex.Message}");
            }
            return RedirectToAction("IndexWarehouseDist", "Distributors", new { id = DetailWarehouse.Warehouses.DistributorId });

            //return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "PowerfulUser,KamAdmin,KamAdCoordinator")]
        public async Task<IActionResult> ProductActivation(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            var DetailWarehouse = await DeatilWarehousesExists(id.Value);
            try
            {
                DeatilWarehouses dwmodel = await _dataContext
                    .DeatilWarehouses
                    .FindAsync(DetailWarehouse.DeatilStoreId);

                dwmodel.IsDeleted = 0;
                dwmodel.RegistrationDate = DateTime.Now.ToUniversalTime();
                _dataContext.DeatilWarehouses.Update(dwmodel);
                await _dataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"The Products can't be deleted because it has related records. {ex.Message}");
            }
            return RedirectToAction("IndexWarehouseDist", "Distributors", new { id = DetailWarehouse.Warehouses.DistributorId });
        }
        private async Task<DeatilWarehouses> DeatilWarehousesExists(long id)
        {
            var _DeatilW = await _dataContext.DeatilWarehouses
                .Where(dw => dw.DeatilStoreId == id)
                .Include(d => d.Warehouses)
                .FirstOrDefaultAsync();
            return _DeatilW;
        }

        [HttpPost]
        public JsonResult OnChangeAutoComplete(string SimTypeId)
        {
            int TypeId = Convert.ToInt32(SimTypeId);
            var _Prod = (from st in _dataContext.Products
                         where st.SimTypeId.Equals(TypeId)
                         select new
                         {
                             ProductId = st.ProductId,
                             Description = st.Description,
                         }).ToList();

            return new JsonResult(new SelectList(_Prod, "ProductId", "Description"));
        }
        public JsonResult GetSubProduct(int SimTypeId)
        {
            List<Products> subProductslist = new List<Products>();

            // ------- Getting Data from Database Using EntityFrameworkCore -------
            subProductslist = (from subcategory in _dataContext.Products
                               where subcategory.SimTypeId == SimTypeId
                               select subcategory).ToList();

            // ------- Inserting Select Item in List -------
            subProductslist.Insert(0, new Products {  ProductId = 0, Description = "Select a product" });


            return Json(new SelectList(subProductslist, "ProductId", "Description"));
        }
    }
}



using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Common.Responses;
using OnlineOrderCart.Web.DataBase;
using OnlineOrderCart.Web.Models;


namespace OnlineOrderCart.Web.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly DataContext _dataContext;
        private readonly IConverterHelper _converterHelper;
        private readonly IConfiguration _configuration;
        private ObservableCollection<UserManagerEntity> MyManger;

        public UserHelper(DataContext dataContext, IConverterHelper converterHelper,
            IConfiguration configuration)
        {
            _dataContext = dataContext;
            _converterHelper = converterHelper;
            _configuration = configuration;
            MyManger = new ObservableCollection<UserManagerEntity>();
        }

        public async Task<Response<Users>> AddUserAsync(AddUserViewModel model, Guid imageId)
        {
            using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = _dataContext.Database.BeginTransaction()) {
                try
                {
                    string NE = $"E{model.EmployeeNumber}";
                    var _register = await _dataContext.Users
                        .Where(u => u.Email.Equals(model.Email) || u.GetKamsCollection.FirstOrDefault().EmployeeNumber.ToUpper()== NE.ToUpper())
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
                    Users _users =  _converterHelper.ToRegisterAsync(model, imageId, true);

                    _dataContext.Users.Add(_users);
                    await _dataContext.SaveChangesAsync();

                    Kams _kam = new Kams {
                        UserId = _users.UserId,
                        EmployeeNumber = model.EmployeeNumber,
                        KamManagerId = model.KamManagerId == 0? null: model.KamManagerId,
                        CodeKey = $"{_users.FirstName.ToUpper().Substring(0,1)}{_users.LastName1.ToUpper().Substring(0, 1)}{_users.LastName2.ToUpper().Substring(0, 1)}",
                        IsDeleted = 100,
                        RegistrationDate = DateTime.Now.ToUniversalTime(),
                    };

                    _dataContext.Kams.Add(_kam);

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
        public Task<Response<UserManagerEntity>> ChangePasswordAsync(UserManagerEntity managerEntity, string OldPassword, string NewPassword)
        {
            throw new NotImplementedException();
        }
        public async Task<Response<object>> ConfirmEmailAsync(Users user, string Jwt, string token){
            using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = _dataContext.Database.BeginTransaction()){
                try{
                    Guid activationCode = new Guid(Jwt);
                    var today = DateTime.Now.ToUniversalTime();

                    var result2 = await (from ua in _dataContext.UserActivations
                                         join u in _dataContext.Users on ua.UserId equals u.UserId
                                         where ua.UserId == user.UserId && ua.JwtId == token
                                         && ua.ActivationCode == activationCode
                                         && ua.ExpiryDate > today
                                         select new{
                                             ua.UserActivationsId,
                                             ua.ActivationCode,
                                             ua.UserId,
                                             ua.JwtId,
                                             ua.ExpiryDate,
                                         }).FirstOrDefaultAsync();

                    if (result2 == null){
                        return new Response<object>
                        {
                            IsSuccess = false,
                            Message = "Operacion fallida  a caducado el registro...",
                        };
                    }
                    UserActivations userActivations = new UserActivations{
                        UserId = user.UserId,
                        ActivationCode = activationCode,
                        JwtId = result2.JwtId,
                        UserActivationsId = result2.UserActivationsId,
                    };

                    _dataContext.Remove(userActivations);
                    await _dataContext.SaveChangesAsync();

                    Users users = await _dataContext
                        .Users
                        .Where(u => u.UserId == user.UserId && u.Email == user.Email && u.UserName.ToUpper() == user.UserName.ToUpper())
                        .FirstOrDefaultAsync();

                    if (users == null){
                        return new Response<object>{
                            IsSuccess = false,
                            Message = "Operacion fallida...",
                        };
                    }

                    users.IsDeleted = 10;
                    _dataContext.Update(users);

                    if (users.IsDistributor != 1){
                        var kam = await _dataContext
                                        .Kams
                                        .Where(k => k.UserId == users.UserId)
                                        .FirstOrDefaultAsync();

                        kam.IsDeleted = 10;
                        _dataContext.Kams.Update(kam);
                    }
                    else {
                        var Dist = await _dataContext
                        .Distributors
                        .Where(k => k.UserId == users.UserId)
                        .FirstOrDefaultAsync();

                        Dist.IsDeleted = 10;
                        _dataContext.Distributors.Update(Dist);
                    }

                    await _dataContext.SaveChangesAsync();

                    transaction.Commit();
                    return new Response<object>{
                        IsSuccess = true,
                    };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new Response<object>{
                        IsSuccess = false,
                        Message = ex.InnerException.Message,
                    };
                }
            }
        }
        public async Task<ObservableCollection<UserManagerEntity>> GetAllKamsRecordsAsync()
        {

            try
            {
                string[] marks = new string[1] { "PowerfulUser" };
                //var  myProducts = await _dataContext
                //                     .RoleGroups
                //                     .Include(r => r.Roles)
                //                     .Include(u => u.Users)
                //                     .ThenInclude(k => k.GetKamsCollection)
                //                     .Where(u => u.Users.IsDeleted == 0).ToListAsync();
                var myProducts = await (from rg in _dataContext.RoleGroups
                                  join r in  _dataContext.Roles on rg.RolId equals r.RolId
                                  join u in _dataContext.Users on rg.UserId equals u.UserId
                                  join k in _dataContext.Kams on u.UserId equals k.UserId
                                  where(u.IsDeleted == 0 && u.IsDistributor == 0 && !marks.Contains(r.RolName))
                                  select new {
                                      UserId = u.UserId,
                                      KamId = k.KamId,
                                      Email = u.Email,
                                      EmployeeNumber = k.EmployeeNumber,
                                      FirstName = u.FirstName,
                                      LastName1 = u.LastName1,
                                      LastName2 = u.LastName2,
                                      GenderId = Convert.ToInt32(u.GenderId),
                                      ImageFullPath = u.ImageFullPath,
                                      PicturePath = u.PicturePath,
                                      KamManagerId = k.KamManagerId,
                                      UserName = u.UserName,
                                      RolName = r.RolName,

                                  }).ToListAsync();

                 this.MyManger = new ObservableCollection<UserManagerEntity>(
                myProducts.Select(p => new UserManagerEntity
                {
                    UserId = p.UserId,
                    KamId = p.KamId,
                    Email = p.Email,
                    EmployeeNumber = p.EmployeeNumber,
                    FirstName = p.FirstName,
                    LastName1 = p.LastName1,
                    LastName2 = p.LastName2,
                    GenderId = Convert.ToInt32(p.GenderId),
                    ImageFullPath = p.ImageFullPath,
                    PicturePath  = p.PicturePath,
                    KamManagerId = p.KamManagerId,
                    Username = p.UserName,
                    RolName = p.RolName,
                    IsAdmin = p.RolName == "KamAdmin" ? true : false,
                })
            .OrderBy(p => p.Username)
            .ToList());
                return MyManger;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<UserManagerEntity> GetUserAsync(string email)
        {
            try
            {
                var _truefalse = await _dataContext
                                    .RoleGroups
                                    .Include(r => r.Roles)
                                    .Include(u => u.Users)
                                    .ThenInclude(u => u.GetKamsCollection)
                                    .Where(u => u.Users.UserName == email && u.IsDeleted == 0).FirstOrDefaultAsync();

                var _manager = new UserManagerEntity
                {
                    UserId = _truefalse.UserId,
                    FirstName = _truefalse.Users.FirstName,
                    LastName1 = _truefalse.Users.LastName1,
                    LastName2 = _truefalse.Users.LastName2,
                    Email = _truefalse.Users.Email,
                    Username = _truefalse.Users.UserName,
                    ImageFullPath = _truefalse.Users.ImageFullPath,
                    ImageId = _truefalse.Users.ImageId,
                    GenderId = Convert.ToInt32(_truefalse.Users.GenderId),
                    RolId = _truefalse.RolId,
                    RolName = _truefalse.Roles.RolName,
                };

                var _result = _truefalse.Users.GetKamsCollection.Where(k => k.UserId == _manager.UserId).FirstOrDefault();
                _manager.KamId = _result.KamId;
                _manager.CodeKey = _result.CodeKey;
                _manager.EmployeeNumber = _result.EmployeeNumber;
                _manager.KamManagerId = _result.KamManagerId == 0 ? 0 : _result.KamManagerId;
                return _manager;
            }
            catch (Exception)
            {

               return null;
            }
        }
        public async Task<Response<UserManagerEntity>> GetUserByEmailAsync(string UserName)
        {
            try
            {
                var _truefalse = await _dataContext
                    .RoleGroups
                    .Include(r => r.Roles)
                    .Include(u => u.Users)
                    .ThenInclude(u => u.GetKamsCollection).Where(u => u.Users.UserName.ToUpper() == UserName.ToUpper() && u.IsDeleted == 0).FirstOrDefaultAsync();
                if (_truefalse == null)
                {
                    return new Response<UserManagerEntity>
                    {
                        IsSuccess = false,
                        Message = "the user data is not correct check the data ....!"
                    };
                }

                var _manager = new UserManagerEntity {
                      UserId = _truefalse.UserId,
                      FirstName = _truefalse.Users.FirstName,
                      LastName1 = _truefalse.Users.LastName1,
                      LastName2 = _truefalse.Users.LastName2,
                      Email = _truefalse.Users.Email,
                      Username = _truefalse.Users.UserName,
                      ImageId = _truefalse.Users.ImageId,
                      PicturePath = _truefalse.Users.ImageFullPath,
                      PictureFullPath = _truefalse.Users.PicturePath,
                      GenderId  = Convert.ToInt32(_truefalse.Users.GenderId),
                      RolId = Convert.ToInt32(_truefalse.RolId),
                };

                var _result = _truefalse.Users.GetKamsCollection.Where(k => k.UserId == _manager.UserId).FirstOrDefault();
                _manager.KamId = _result.KamId;
                _manager.EmployeeNumber = _result.EmployeeNumber;
                _manager.CodeKey = _result.CodeKey;
                _manager.KamManagerId = _result.KamManagerId == 0 ? 0 : _result.KamManagerId;
                return new Response<UserManagerEntity>
                {
                    IsSuccess = true,
                    Message = "Ok ....!",
                    Result = _manager,
                };
            }

            catch (Exception ex)
            {
                return new Response<UserManagerEntity>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<Response<Users>> GetUserByIdAsync(string id)
        {
            try
            {
                long _id = Convert.ToInt64(id);

                Users _Users = await _dataContext
                    .Users
                    .Where(u => u.UserId == _id)
                    .FirstOrDefaultAsync();

                if (_Users == null)
                {
                    return new Response<Users>
                    {
                        IsSuccess = false,
                    };
                }
                return new Response<Users>
                {
                    IsSuccess = true,
                    Result = _Users,
                };
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message.Contains("duplicate"))
                {
                    return new Response<Users>
                    {
                        IsSuccess = false,
                        Message = "Already there is a record with the same name.",
                    };
                }
                else
                {
                    return new Response<Users>
                    {
                        IsSuccess = false,
                        Message = ex.InnerException.Message,
                    };
                }
            }
        }
        public async Task<Response<UserManagerEntity>> GetKamByIdAsync(long id)
        {
            try
            {
               var _Users = await _dataContext
                    .Kams
                    .Include(k => k.Users)
                    .Where(k => k.IsDeleted == 0 && k.KamId == id)
                    .FirstOrDefaultAsync();

                if (_Users == null)
                {
                    return new Response<UserManagerEntity>
                    {
                        IsSuccess = false,
                    };
                }

                var model = new UserManagerEntity {
                    UserId = _Users.UserId,
                    KamId =_Users.KamId,
                    FirstName = _Users.Users.FirstName,
                    LastName1 = _Users.Users.LastName1,
                    LastName2 = _Users.Users.LastName2,
                    Email = _Users.Users.Email,
                    Username = _Users.Users.UserName,
                    EmployeeNumber = _Users.EmployeeNumber,
                };

                return new Response<UserManagerEntity>
                {
                    IsSuccess = true,
                    Result = model,
                };
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message.Contains("duplicate"))
                {
                    return new Response<UserManagerEntity>
                    {
                        IsSuccess = false,
                        Message = "Already there is a record with the same name.",
                    };
                }
                else
                {
                    return new Response<UserManagerEntity>
                    {
                        IsSuccess = false,
                        Message = ex.InnerException.Message,
                    };
                }
            }
        }
        public async Task<Response<RUKDViewModel>> GetValidateLoginAsync(LoginViewModel model){
            try
            {
                var resetPassword = _converterHelper.CreateSHA256(_configuration["SecretP:SecretPassword"]);

                var _password = _converterHelper.CreateSHA256(model.Password);

                var _truefalse =  await _dataContext.Users.Where(u => u.UserName.ToUpper() == model.Username.ToUpper() && u.IsDeleted==0).AnyAsync();
                if (!_truefalse)
                {
                    if (_password.Message == resetPassword.Message)
                    {
                        return new Response<RUKDViewModel>
                        {
                            IsSuccess = false,
                            Message = "ErrorPassword!"
                        };
                    }

                    return new Response<RUKDViewModel> {
                        IsSuccess = false,
                        Message = "the user data is not correct check the data ....!"
                    };
                }

                var newusers = await _dataContext
                    .Users
                    .Where(u => u.UserName == model.Username && u.Password == _password.Message && u.IsDeleted == 0 )
                    .FirstOrDefaultAsync();

                if (newusers == null){
                    return new Response<RUKDViewModel>
                    {
                        IsSuccess = false,
                        Message = "Data is Incorrect in the user or  password wrong........",
                    };
                }

                RUKDViewModel _object = new RUKDViewModel();

                if (newusers != null && newusers.IsDistributor != 1){
                    var _KUsers = await _dataContext
                                       .RoleGroups
                                       .Include(r => r.Roles)
                                       .Include(u => u.Users)
                                       .ThenInclude(u => u.GetKamsCollection)
                                       .Where(r => r.Users.UserName.Equals(model.Username) && r.Users.Password.Equals(_password.Message) && r.Users.IsDeleted == 0 && r.Users.IsDistributor == 0)
                                       .FirstOrDefaultAsync();

                    _object.MyFallName = $"{_KUsers.Users.FirstName} {_KUsers.Users.LastName1} {_KUsers.Users.LastName2}";
                    _object.RolName = _KUsers.Roles.RolName;
                    _object.RolId = _KUsers.RolId;
                    _object.UserId = _KUsers.UserId;
                    _object.RoleGroupId = _KUsers.RoleGroupId;
                    _object.UserName = _KUsers.Users.UserName;
                }
                else {
                    //var _DUsers = await _dataContext.RoleGroups
                    //                   .Include(r => r.Roles)
                    //                   .Include(u => u.Users)
                    //                   .ThenInclude(u => u.GetKamsCollection)
                    //                   .Include(u => u.Users.GetKamsCollection)
                    //                   .ThenInclude(d => d.GetDistributorCollection.)

                    ///     .Where(u => u.Users.UserName.Equals(model.Username) && u.Users.Password.Equals(_password.Message) && u.Users.IsDeleted == 0 && u.Users.IsDistributor == 1)
                    ///     .FirstOrDefaultAsync();
                    ///     

                    var _DUsers = await (from rg in _dataContext.RoleGroups
                                   join r in _dataContext.Roles on rg.RolId equals r.RolId
                                   join u in _dataContext.Users on rg.UserId equals u.UserId
                                   join d in _dataContext.Distributors on u.UserId equals d.UserId
                                   where(u.UserName == model.Username && u.Password.Equals(_password.Message) && u.IsDeleted == 0 && u.IsDistributor == 1)
                                   select new {
                                    UserId  = u.UserId,
                                    UserName = u.UserName,
                                    RolName = r.RolName,
                                    KamId = d.KamId,
                                    RolId = r.RolId,
                                    RoleGroupId = rg.RoleGroupId,
                                   }).FirstOrDefaultAsync();

                    if (_DUsers == null)
                    {
                        return new Response<RUKDViewModel>
                        {
                            IsSuccess = false,
                            Message = "Error Distributor ........",
                        };
                    }

                    var _Ku = (from u in _dataContext.Users
                               join k in _dataContext.Kams on u.UserId equals k.UserId
                               where(k.KamId == _DUsers.KamId)
                               select new {
                                   FirstName = u.FirstName,
                                   LastName1 = u.LastName1,
                                   LastName2 = u.LastName2,
                               }).FirstOrDefault();

                    _object.MyFallName = $"{_Ku.FirstName} {_Ku.LastName1} {_Ku.LastName2}";
                    _object.RolName = _DUsers.RolName;
                    _object.RolId = _DUsers.RolId;
                    _object.UserId = _DUsers.UserId;
                    _object.RoleGroupId = _DUsers.RoleGroupId;
                    _object.UserName = _DUsers.UserName;

                }

                return new Response<RUKDViewModel>
                {
                    IsSuccess = true,
                    Message = "Ok",
                    Result = _object
                };

            }
            catch (Exception ex)
            {
                return new Response<RUKDViewModel>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<Response<UserManagerEntity>> ValidatePasswordAsync(UserManagerEntity user, string Password)
        {
            try
            {
                var _password = _converterHelper.CreateSHA256(Password);
                var _truefalse = await _dataContext
                    .RoleGroups
                    .Include(r => r.Roles)
                    .Include(u => u.Users)
                    .ThenInclude(u => u.GetKamsCollection).Where(u => u.Users.UserName.ToUpper() == user.Username.ToUpper() && u.Users.Password.Equals(_password) && u.IsDeleted == 0).FirstOrDefaultAsync();
                if (_truefalse == null)
                {
                    return new Response<UserManagerEntity>
                    {
                        IsSuccess = false,
                        Message = "the user data is not correct check the data ....!"
                    };
                }

                var _manager = new UserManagerEntity
                {
                    UserId = _truefalse.UserId,
                    FirstName = _truefalse.Users.FirstName,
                    LastName1 = _truefalse.Users.LastName1,
                    LastName2 = _truefalse.Users.LastName2,
                    Email = _truefalse.Users.Email,
                    Username = _truefalse.Users.UserName,
                    ImageId = _truefalse.Users.ImageId,
                    PicturePath = _truefalse.Users.ImageFullPath,
                    GenderId = Convert.ToInt32(_truefalse.Users.GenderId),
                };

                var _result = _truefalse.Users.GetKamsCollection.Where(k => k.UserId == _manager.UserId).FirstOrDefault();
                _manager.KamId = _result.KamId;
                _manager.EmployeeNumber = _result.EmployeeNumber;
                _manager.KamManagerId = _result.KamManagerId == 0 ? 0 : _result.KamManagerId;
                return new Response<UserManagerEntity>
                {
                    IsSuccess = true,
                    Message = "Ok ....!",
                    Result = _manager,
                };
            }

            catch (Exception ex)
            {
                return new Response<UserManagerEntity>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<Response<object>> GetConfirmPasswordAsync(ConfirmPasswordViewModel model)
        {
            using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = _dataContext.Database.BeginTransaction()) { 
            
                try
                {
                    var Secret = _converterHelper.CreateSHA256(_configuration["SecretP:SecretPassword"]);
                    var oldPassword = _converterHelper.CreateSHA256(model.OldPassword);
                    if (Secret.Message != oldPassword.Message)
                    {
                        return new Response<object>
                        {
                            IsSuccess = false,
                            Message = "the old password does not correspond to the data entered",
                        };
                    }
                    var Password = _converterHelper.CreateSHA256(model.NewPassword);
                    var _users = await _dataContext
                        .Users
                        .Where(u => u.UserName.ToUpper() == model.UserName.ToUpper() && u.IsDeleted != 0)
                        .FirstOrDefaultAsync();
                    if (_users == null)
                    {
                        return new Response<object>
                        {
                            IsSuccess = false,
                            Message = "the user data is not correct check the data ....!"
                        };
                    }

                    _users.Password = Password.Message;
                    _users.IsDeleted = 0;
                    _dataContext.Users.Update(_users);

                    if (_users.IsDistributor != 1)
                    {
                        var kam = await _dataContext
                                        .Kams
                                        .Where(k => k.UserId == _users.UserId)
                                        .FirstOrDefaultAsync();

                        kam.IsDeleted = 0;
                        _dataContext.Kams.Update(kam);
                    }
                    else
                    {
                        var Dist = await _dataContext
                        .Distributors
                        .Where(k => k.UserId == _users.UserId)
                        .FirstOrDefaultAsync();

                        Dist.IsDeleted = 0;
                        _dataContext.Distributors.Update(Dist);
                    }
                   await _dataContext.SaveChangesAsync();
                    transaction.Commit();
                    return new Response<object>
                    {
                        IsSuccess = true,
                        Message = "operation carried out successfully",
                    };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new Response<object>
                    {
                        IsSuccess = false,
                        Message = ex.Message,
                        Result = ex,
                    };
                }
           }
        }
        public async Task<Response<object>> GetAllAvatarConfirmAsync(string UserName)
        {
            try
            {
              var _DistKams = await _dataContext
                    .Users
                    .Where(u => u.UserName == UserName && u.IsDeleted != 0)
                    .FirstOrDefaultAsync();
                if (_DistKams == null){
                    return new Response<object>
                    {
                        IsSuccess = false,
                        Message = "Information on the requested record is not found..!",
                    };
                }

                return new Response<object>
                {
                    IsSuccess = true,
                    Message = "Information on the requested record is found..!",
                };

            }
            catch (Exception ex)
            {
                return new Response<object>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
        public async Task<Response<Users>> GetUserIdByUsernameAsync(string username, string id)
        {
            try{

                var _Users = await _dataContext
                    .Users
                    .Where(u => u.UserName == username)
                    .FirstOrDefaultAsync();

                if (_Users == null)
                {
                    return new Response<Users>
                    {
                        IsSuccess = false,
                        Message = "the data is not correct please check it thanks ....",
                    };
                }

                return new Response<Users>
                {
                    IsSuccess = true,
                    Result = _Users,
                };
            }
            catch (Exception ex){
                return new Response<Users>{
                    IsSuccess = false,
                    Message = ex.InnerException.Message,
                };
            }
        }
    }
}

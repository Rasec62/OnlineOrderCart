using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Common.Responses;
using OnlineOrderCart.Web.DataBase;
using OnlineOrderCart.Web.Helpers;
using OnlineOrderCart.Web.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Vereyon.Web;

namespace OnlineOrderCart.Web.Controllers
{
    [Authorize(Roles = "PowerfulUser,KamAdmin,KamAdCoordinator")]
    public class CoordinatorController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;
        private readonly IConfiguration _configuration;
        private readonly ICombosHelper _combosHelper;
        private readonly IMovementsHelper _movementsHelper;
        private readonly IFlashMessage _flashMessage;
        private readonly IImageHelper _imageHelper;
        private readonly IMailHelper _mailHelper;

        public CoordinatorController(DataContext dataContext
            , IUserHelper userHelper, IConfiguration configuration,
            ICombosHelper combosHelper, IMovementsHelper movementsHelper,
            IFlashMessage flashMessage, IMailHelper mailHelper, IImageHelper ImageHelper)
        {
            _dataContext = dataContext;
            _userHelper = userHelper;
            _configuration = configuration;
            _combosHelper = combosHelper;
            _movementsHelper = movementsHelper;
            _flashMessage = flashMessage;
            _imageHelper = ImageHelper;
            _mailHelper = mailHelper;
        }
        public async Task<IActionResult> Index()
        {
            if (User.Identity.Name == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            var ListAsync = await _userHelper.GetAllCoordRecordsAsync();

            return View(ListAsync);
        }
        [HttpGet]
        public async Task<IActionResult> CreateCoordinator()
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

            var model = new AddUserViewModel
            {
                Password = _configuration["SecretP:SecretPassword"],
                PasswordConfirm = _configuration["SecretP:SecretPassword"],
                ComboGender = _combosHelper.GetComboGenders(),
                ComboRoles = _combosHelper.GetComboCoordRoles(),
                ComboKams = _movementsHelper.GetSqlComboAllKams(),
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCoordinator(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
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
                    Guid imageId = Guid.Empty;
                    string Path = string.Empty;
                    model.KamManagerId = model.KamId;
                    model.IsCoordinator = 1;
                    if (model.ImageFile != null)
                    {
                        //imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                        model.PicturePath = await _imageHelper.UploadImageAsync(model.ImageFile, "users");
                    }

                    Response<Users> user = await _userHelper.AddUserAsync(model, imageId);
                    if (user.IsSuccess == false)
                    {
                        _flashMessage.Danger(string.Empty, user.Message);
                        ModelState.AddModelError(string.Empty, user.Message);
                        model.ComboGender = _combosHelper.GetComboGenders();
                        model.ComboRoles = _combosHelper.GetComboRoles();
                        return View(model);
                    }

                    TokenResponse myToken = GetToken(user.Result.Email);

                    Guid activationCode = Guid.NewGuid();
                    var userActivations = new UserActivations
                    {
                        ActivationCode = activationCode,
                        UserId = user.Result.UserId,
                        UserName = user.Result.UserName,
                        EventAction = "ConfirmEmail - CoordRegister",
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

                    Response<object> response = _mailHelper.SendMail(model.Email, $"Email confirmation. this is your User Name :{user.Result.UserName} and Temporal password :{Password}", $"<h1>Email Confirmation</h1>" +
                        $"To allow the user, " +
                        $"plase click in this link:<p><a href = \"{tokenLink}\" style='color: #8ebf42'>Confirm Email . this is your User Name :{user.Result.UserName} and  Temporal Password :{Password}</a></p>");
                    if (response.IsSuccess)
                    {
                        ViewBag.Message = "The instructions to allow your user has been sent to email.";
                        _flashMessage.Confirmation("The instructions to allow your user has been sent to email.");
                        return RedirectToAction("Index", "Coordinator");
                    }

                    ModelState.AddModelError(string.Empty, response.Message);
                    _flashMessage.Danger(response.Message, "This Confirm Email is already used.");

                }
                catch (System.Exception ex)
                {

                    _flashMessage.Danger("This Confirm Error is already used.", ex.Message);
                    model.ComboGender = _combosHelper.GetComboGenders();
                    model.ComboRoles = _combosHelper.GetComboCoordRoles();
                    model.ComboKams = _movementsHelper.GetSqlComboAllKams();
                    return View(model);
                }
            }
            model.ComboGender = _combosHelper.GetComboGenders();
            model.ComboRoles = _combosHelper.GetComboCoordRoles();
            model.ComboKams = _movementsHelper.GetSqlComboAllKams();
            return View(model);
        }
        public async Task<IActionResult> DeleteRegister(int? id)
        {
            if (User.Identity.Name == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            try
            {
                var kam = await CoordExists(id.Value);

                if (kam == null)
                {
                    return new NotFoundViewResult("_ResourceNotFound");
                }

                kam.IsDeleted = 1;
                _dataContext.Kams.Update(kam);
                await _dataContext.SaveChangesAsync();
                var _user = await _dataContext.Users.FindAsync(kam.UserId);
                if (_user == null)
                {
                    return new NotFoundViewResult("_ResourceNotFound");
                }
                _user.IsDeleted = 1;
                await _dataContext.SaveChangesAsync();
                _flashMessage.Confirmation("The record was deleted.");
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"The record can't be deleted because it has related records. {ex.Message}");
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> EditCoordinator(int? id)
        {
            if (User.Identity.Name == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }


            UserManagerEntity user = await _userHelper.GetUserAsync(User.Identity.Name);
            if (user == null)
            {
                return new NotFoundViewResult("_ResourceNotFound"); ;
            }
            var _users = await _userHelper.GetCoordByIdAsync(id.Value);
            if (!_users.IsSuccess)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            var _managerIndexs = await _userHelper.GetCoordByEmailAsync(_users.Result.Username);

            if (_managerIndexs == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            var model = new AddUserViewModel
            {
                UserId = _managerIndexs.Result.UserId,
                KamId = _managerIndexs.Result.KamId,
                FirstName = _managerIndexs.Result.FirstName,
                LastName1 = _managerIndexs.Result.LastName1,
                LastName2 = _managerIndexs.Result.LastName2,
                GenderId = _managerIndexs.Result.GenderId,
                Email = _managerIndexs.Result.Email,
                Username = _managerIndexs.Result.Username,
                RolId = _managerIndexs.Result.RolId,
                PicturePath = _managerIndexs.Result.PicturePath,
                PictureFPath = _managerIndexs.Result.PictureFPath,
                EmployeeNumber = _managerIndexs.Result.EmployeeNumber,
                CodeKey = _managerIndexs.Result.CodeKey,
                KamManagerId = _managerIndexs.Result.KamManagerId,
                Password = "D*12345678",
                PasswordConfirm = "D*12345678",
            };

            model.ComboGender = _combosHelper.GetComboGenders();
            model.ComboRoles = _combosHelper.GetComboCoordRoles();
            model.ComboKams = _movementsHelper.GetSqlComboAllKams();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCoordinator(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
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
                           .Where(u => u.UserId == model.UserId && u.UserName == model.Username && u.IsDistributor == 0)
                           .FirstOrDefaultAsync();

                        if (_users == null)
                        {
                            return new NotFoundViewResult("_ResourceNotFound");
                        }
                        _users.FirstName = model.FirstName ?? _users.FirstName;
                        _users.LastName1 = model.LastName1 ?? _users.LastName1;
                        _users.LastName2 = model.LastName2 ?? _users.LastName2;
                        _users.GenderId = model.GenderId.ToString() ?? _users.GenderId;
                        _users.Email = model.Email ?? _users.Email;
                        _users.UserName = model.Username ?? _users.UserName;
                        _users.ImageId = imageId;
                        _users.PicturePath = model.PicturePath ?? path;

                        _dataContext.Users.Update(_users);

                        Kams _Coord = await _dataContext
                            .Kams
                            .Where(k => k.EmployeeNumber == model.EmployeeNumber.ToString() && k.IsCoordinator.Equals(1) && k.UserId == _users.UserId && k.KamId.Equals(model.KamId))
                            .FirstOrDefaultAsync();

                        if (_Coord == null)
                        {
                            return new NotFoundViewResult("_ResourceNotFound");
                        }
                        _Coord.EmployeeNumber = model.EmployeeNumber ?? _Coord.EmployeeNumber;
                        _Coord.CodeKey = model.CodeKey ?? _Coord.CodeKey;
                        _Coord.KamManagerId = model.KamManagerId != _Coord.KamManagerId ? model.KamManagerId : _Coord.KamManagerId;

                        _dataContext.Update(_Coord);
                        await _dataContext.SaveChangesAsync();
                        transaction.Commit();
                        return RedirectToAction("Index", "Coordinator");

                    }
                    catch (DbUpdateException dbUpdateException)
                    {
                        transaction.Rollback();
                        if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                        {
                            ModelState.AddModelError(string.Empty, "Ya existe un Distribuidor.");
                            _flashMessage.Danger(string.Empty, "Ya existe un Distribuidor.");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                            _flashMessage.Danger(string.Empty, dbUpdateException.InnerException.Message);
                        }
                        model.ComboGender = _combosHelper.GetComboGenders();
                        model.ComboRoles = _combosHelper.GetComboCoordRoles();
                        model.ComboKams = _movementsHelper.GetSqlComboAllKams();

                        return View(model);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        model.ComboGender = _combosHelper.GetComboGenders();
                        model.ComboRoles = _combosHelper.GetComboCoordRoles();
                        model.ComboKams = _movementsHelper.GetSqlComboAllKams();
                        _flashMessage.Danger("", ex.Message);
                        return View(model);
                    }
                }
            }
            model.ComboGender = _combosHelper.GetComboGenders();
            model.ComboRoles = _combosHelper.GetComboCoordRoles();
            model.ComboKams = _movementsHelper.GetSqlComboAllKams();
            return View(model);
        }
        public async Task<IActionResult> DetailCoordinator(long? id)
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
            var ListAsync = await _userHelper.GetAllCoordRecordsAsync();

            var model = ListAsync.Where(x => x.KamId == id && x.IsCoordinator == 1).FirstOrDefault();
            return View(model);

        }
        private TokenResponse GetToken(string email)
        {
            Claim[] claims = new[]{
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:SecretKey"]));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token = new JwtSecurityToken(
                _configuration["Tokens:Issuer"],
                _configuration["Tokens:Audience"],
                claims,
                expires: DateTime.UtcNow.AddDays(2),
                signingCredentials: credentials);
            return new TokenResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo
            };
        }
        private async Task<Kams> CoordExists(int id)
        {
            var _kam = await _dataContext.Kams.FindAsync(id);
            return _kam;
        }
    }
}

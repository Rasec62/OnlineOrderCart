using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Vereyon.Web;

namespace OnlineOrderCart.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConverterHelper _converterHelper;
        private readonly IUserHelper _userHelper;
        private readonly IFlashMessage _flashMessage;
        private readonly IConfiguration _configuration;
        private readonly ICombosHelper _combosHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly DataContext _dataContext;
        private readonly IMovementsHelper _movementsHelper;
        private readonly IImageHelper _imageHelper;

        public AccountController(IConverterHelper converterHelper,
            IUserHelper userHelper, IFlashMessage flashMessage,
            IConfiguration configuration, ICombosHelper combosHelper
            , IMailHelper mailHelper, IBlobHelper blobHelper,
            DataContext dataContext, IMovementsHelper movementsHelper, IImageHelper ImageHelper)
        {
            _converterHelper = converterHelper;
            _userHelper = userHelper;
            _flashMessage = flashMessage;
            _configuration = configuration;
            _combosHelper = combosHelper;
            _mailHelper = mailHelper;
            _blobHelper = blobHelper;
            _dataContext = dataContext;
            _movementsHelper = movementsHelper;
            _imageHelper = ImageHelper;
        }
        [Authorize(Roles = "PowerfulUser,KamAdmin,KamAdCoordinator")]
        public async Task<IActionResult> IndexRegister()
        {

            var ListAsync = await _userHelper.GetAllKamsRecordsAsync();

            return View(ListAsync);
        }
        public IActionResult ResourceNotFound()
        {
            return this.View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied(string returnUrl)
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            Response<RUKDViewModel> _result = new Response<RUKDViewModel>();
            if (ModelState.IsValid)
            {

                _result = await _userHelper.GetValidateLoginAsync(model);

                if (!_result.IsSuccess)
                {
                    if (_result.Message == "ErrorPassword!")
                    {
                        return RedirectToAction("ConfirmPassword", "Account");
                    }
                    ModelState.AddModelError(string.Empty, $"{_result.Message}");
                    _flashMessage.Danger(_result.Message, "Incorrect information check");
                    return View(model);
                }

                var RolUser = (RUKDViewModel)_result.Result;

                var claims = new List<Claim>
                    {
                        new Claim("user", RolUser.UserName),
                        new Claim("role", RolUser.RolName),
                        new Claim("UserId", RolUser.UserId.ToString()),
                    };

                await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme, "user", "role")));

                if (Request.Query.Keys.Contains("ReturnUrl"))
                {
                    return Redirect(Request.Query["ReturnUrl"].First());
                }

                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError(string.Empty, "Failed to login.");
            return View(model);
        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "PowerfulUser,KamAdmin,KamAdCoordinator")]
        public async Task<IActionResult> ChangeUser()
        {
            if (User.Identity.Name == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            UserManagerEntity user = await _userHelper.GetUserAsync(User.Identity.Name);
            if (user == null)
            {
                return new NotFoundViewResult("_ResourceNotFound"); ;
            }

            AddUserViewModel model = new AddUserViewModel
            {
                FirstName = user.FirstName,
                LastName1 = user.LastName1,
                LastName2 = user.LastName2,
                ImageId = user.ImageId,
                PicturePath = user.ImageFullPath,
                UserId = user.UserId,
                KamId = user.KamId,
                KamManagerId = user.KamManagerId,
                GenderId = user.GenderId,
                RolId = user.RolId,
                Email = user.Email,
                Username = user.Username,
                EmployeeNumber = user.EmployeeNumber,
                ComboGender = _combosHelper.GetComboGenders(),
                ComboRoles = _combosHelper.GetComboRoles(),
                Password = "1234567890",
                PasswordConfirm = "1234567890",

            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUser(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                string path = model.PicturePath;

                Guid imageId = model.ImageId;

                if (model.ImageFile != null)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                }
                using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = _dataContext.Database.BeginTransaction())
                {
                    try
                    {
                        //Users _users = await _converterHelper.ToSRegisterAsync(model, imageId, false);

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

                        _dataContext.Users.Update(_users);

                        Kams _kam = new Kams
                        {
                            UserId = _users.UserId,
                            KamId = model.KamId,
                            EmployeeNumber = model.EmployeeNumber,
                            KamManagerId = model.KamManagerId == 0 ? null : model.KamManagerId,
                            CodeKey = $"{_users.FirstName.ToUpper().Substring(0, 1)}{_users.LastName1.ToUpper().Substring(0, 1)}{_users.LastName2.ToUpper().Substring(0, 1)}",
                            IsDeleted = 0,
                            RegistrationDate = DateTime.Now.ToUniversalTime(),
                        };
                        _dataContext.Kams.Update(_kam);
                        await _dataContext.SaveChangesAsync();
                        transaction.Commit();
                        return RedirectToAction("Index", "Home");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        model.ComboGender = _combosHelper.GetComboGenders();
                        model.ComboRoles = _combosHelper.GetComboRoles();
                        _flashMessage.Danger(ex.Message, "This email is already used.");
                        return View(model);
                    }
                }
            }
            model.ComboGender = _combosHelper.GetComboGenders();
            model.ComboRoles = _combosHelper.GetComboRoles();
            return View(model);
        }

        [Authorize(Roles = "PowerfulUser,KamAdmin,KamAdCoordinator")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                UserManagerEntity user = await _userHelper.GetUserAsync(User.Identity.Name);
                if (user != null)
                {
                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.IsSuccess)
                    {
                        return RedirectToAction("ChangeUser");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Message);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User no found.");
                }
            }

            return View(model);
        }

        public IActionResult RecoverPasswordMVC()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RecoverPasswordMVC(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "The email doesn't correspont to a registered user.");
                    return View(model);
                }

                var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user.Result);
                Guid activationCode = Guid.NewGuid();
                var TblResetP = new TblResetPasswords
                {
                    Id = Guid.NewGuid(),
                    UserName = user.Result.Username,
                    Jwt = activationCode.ToString(),
                    Token = myToken.Result.Token,
                    ExpirationDate = myToken.Result.Expiration.ToUniversalTime(),
                    IsDeleted = 10,
                    RegistrationDate = DateTime.Now.ToUniversalTime(),
                };

                _dataContext.TblResetPasswords.Add(TblResetP);
                await _dataContext.SaveChangesAsync();

                var link = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { PostalCode = user.Result.Username, Jwt = activationCode, token = myToken.Result.Token }, protocol: HttpContext.Request.Scheme);

                Response<object> response = _mailHelper.SendMail(user.Result.Email, "Shopping Cart System Password Reset", $"<h1>Shopping Cart System Password Reset</h1>" +
                    $"To reset the password click in this link:</br></br>" +
                    $"<a href = \"{link}\">Reset Password</a>");
                if (response.IsSuccess)
                {
                    ViewBag.Message = "The instructions to recover your password has been sent to email.";
                    _flashMessage.Confirmation("The instructions to allow your user has been sent to email.");
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                    _flashMessage.Danger(string.Empty, response.Message);
                }

                return View();

            }

            return View(model);
        }
        public IActionResult ResetPassword(string PostalCode, string Jwt, string token)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(Jwt) || string.IsNullOrEmpty(PostalCode))
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            var _MyRPToken = _dataContext
                .TblResetPasswords
                .FirstOrDefaultAsync(x => x.Jwt == Jwt && x.Token == token && x.UserName == PostalCode && x.IsDeleted == 10);
            if (_MyRPToken == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            var model = new ResetPasswordViewModel { Token = token, Jwt = Jwt, PostalCode = PostalCode };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.UserName);
            if (user != null)
            {
                var result = await _userHelper.ResetPasswordAsync(user.Result, model.Jwt, model.Token, model.Password);
                if (result.IsSuccess)
                {
                    ViewBag.Message = "Password reset successful.";
                    _flashMessage.Confirmation("", "Password reset successful.");
                    return RedirectToAction("Login", "Account");
                }

                ViewBag.Message = "Error while resetting the password.";
                return View(model);
            }

            ViewBag.Message = "User not found.";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);
                if (user != null)
                {
                    var result = await _userHelper.ValidatePasswordAsync(
                        user.Result,
                        model.Password);

                    if (result.IsSuccess)
                    {
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, result.Result.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
                        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(
                            _configuration["Tokens:Issuer"],
                            _configuration["Tokens:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddMonths(6),
                            signingCredentials: credentials);
                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };

                        return Created(string.Empty, results);
                    }
                }
            }

            return BadRequest();
        }

        [HttpGet]
        [Authorize(Roles = "PowerfulUser,KamAdmin,KamAdCoordinator")]
        public async Task<IActionResult> Register()
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
                ComboRoles = _combosHelper.GetComboRoles(),
                //ComboKams = _combosHelper.GetComboKams(),
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AddUserViewModel model)
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

                    switch (model.RolId)
                    {
                        case 2:
                            model.KamManagerId = 0;
                            model.IsCoordinator = 0;
                            break;
                        case 3:
                            model.KamManagerId = _users.Result.KamId;
                            model.IsCoordinator = 1;
                            break;
                        case 4:
                            model.KamManagerId = 0;
                            model.IsCoordinator = 0;
                            break;
                    }

                    Guid imageId = Guid.Empty;
                    string Path = string.Empty;

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
                        EventAction = "ConfirmEmail - KamRegister",
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
                        return RedirectToAction("IndexRegister", "Account");
                    }

                    ModelState.AddModelError(string.Empty, response.Message);
                    _flashMessage.Danger(string.Empty, response.Message);

                }
                catch (Exception ex)
                {

                    _flashMessage.Danger(ex.Message, "This Confirm Email is already used.");
                    model.ComboGender = _combosHelper.GetComboGenders();
                    model.ComboRoles = _combosHelper.GetComboRoles();
                    //model.ComboKams = _combosHelper.GetComboKams();
                    return View(model);
                }
            }
            model.ComboGender = _combosHelper.GetComboGenders();
            model.ComboRoles = _combosHelper.GetComboRoles();
            //model.ComboKams = _combosHelper.GetComboKams();
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "PowerfulUser,KamAdmin,KamAdCoordinator")]
        public async Task<IActionResult> EditRegister(int? id)
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
            var _users = await _userHelper.GetKamByIdAsync(id.Value);
            if (!_users.IsSuccess)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            var _managerIndexs = await _userHelper.GetUserByEmailAsync(_users.Result.Username);

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
                PictureFPath = _managerIndexs.Result.PictureFullPath,
                EmployeeNumber = _managerIndexs.Result.EmployeeNumber,
                CodeKey = _managerIndexs.Result.CodeKey,
                KamManagerId = _managerIndexs.Result.KamManagerId,
                Password = "D*12345678",
                PasswordConfirm = "D*12345678",
            };

            model.ComboGender = _combosHelper.GetComboGenders();
            model.ComboRoles = _combosHelper.GetComboRoles();
            model.ComboKams = _movementsHelper.GetSqlDataKams();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRegister(AddUserViewModel model)
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
                model.PicturePath = path;
                using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = _dataContext.Database.BeginTransaction())
                {
                    try
                    {
                        //Users _users = await _converterHelper.ToSRegisterAsync(model, imageId, false);

                        Users _users = await _dataContext
                           .Users
                           .Where(u => u.UserId == model.UserId && u.UserName == model.Username && u.IsDistributor == 0)
                           .FirstOrDefaultAsync();

                        _users.FirstName = model.FirstName ?? _users.FirstName;
                        _users.LastName1 = model.LastName1 ?? _users.LastName1;
                        _users.LastName2 = model.LastName2 ?? _users.LastName2;
                        _users.GenderId = model.GenderId.ToString() ?? _users.GenderId;
                        _users.Email = model.Email ?? _users.Email;
                        _users.UserName = model.Username ?? _users.UserName;
                        _users.ImageId = imageId;
                        _users.PicturePath = model.PicturePath;

                        _dataContext.Users.Update(_users);

                        Kams _kams = await _dataContext
                            .Kams
                            .Where(k => k.KamId == model.KamId && k.UserId == _users.UserId && k.IsCoordinator == 0)
                            .FirstOrDefaultAsync();

                        if (_kams == null)
                        {
                            return new NotFoundViewResult("_ResourceNotFound");
                        }

                        _kams.EmployeeNumber = model.EmployeeNumber ?? _kams.EmployeeNumber;
                        _kams.KamManagerId = model.KamManagerId == 0 ? null : _kams.KamManagerId;
                        _kams.CodeKey = model.CodeKey ?? $"{_users.FirstName.ToUpper().Substring(0, 1)}{_users.LastName1.ToUpper().Substring(0, 1)}{_users.LastName2.ToUpper().Substring(0, 1)}";

                        _dataContext.Kams.Update(_kams);
                        await _dataContext.SaveChangesAsync();
                        transaction.Commit();
                        return RedirectToAction("Index", "Home");
                    }
                    catch (DbUpdateException dbUpdateException)
                    {
                        transaction.Rollback();
                        if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                        {
                            ModelState.AddModelError(string.Empty, "Ya existe un vehículo con esa placa.");
                            _flashMessage.Danger(string.Empty, "Ya existe un Usuario.");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                            _flashMessage.Danger(string.Empty, dbUpdateException.InnerException.Message);
                        }

                        model.ComboGender = _combosHelper.GetComboGenders();
                        model.ComboRoles = _combosHelper.GetComboRoles();
                        model.ComboKams = _movementsHelper.GetSqlDataKams();

                        return View(model);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        model.ComboGender = _combosHelper.GetComboGenders();
                        model.ComboRoles = _combosHelper.GetComboRoles();
                        model.ComboKams = _movementsHelper.GetSqlDataKams();
                        _flashMessage.Danger(ex.Message, "This email is already used.");
                        return View(model);
                    }
                }
            }
            model.ComboGender = _combosHelper.GetComboGenders();
            model.ComboRoles = _combosHelper.GetComboRoles();
            model.ComboKams = _movementsHelper.GetSqlDataKams();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string username, string Jwt, string token)
        {

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(Jwt))
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            var user = await _userHelper.GetUserIdByUsernameAsync(username, userId);
            if (user == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            var result = await _userHelper.ConfirmEmailAsync(user.Result, Jwt, token);
            if (!result.IsSuccess)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);
            return View();
        }

        [HttpGet]
        public IActionResult ConfirmPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPassword(ConfirmPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var _users = await _userHelper.GetAllAvatarConfirmAsync(model.UserName);

                if (_users.IsSuccess == false)
                {
                    _flashMessage.Danger(_users.Message, "Incorrect information check");
                    return View(model);
                }
                var _result = await _userHelper.GetConfirmPasswordAsync(model);
                if (_result.IsSuccess)
                {
                    _flashMessage.Confirmation(_result.Message, "Correct information check");
                    return RedirectToAction("Login", "Account");
                }
                _flashMessage.Danger(_result.Message, "Incorrect information check");
            }
            _flashMessage.Danger("no Data.....", "Incorrect information check");
            return View(model);
        }

        private TokenResponse GetToken(string email)
        {
            Claim[] claims = new[]
            {
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
        public string GenerateJWTToken(Users userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Email),
                new Claim("fullName", $"{userInfo.FirstName.ToString()}{userInfo.LastName1.ToString()}"),
                new Claim("FolioNumber",userInfo.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Tokens:Issuer"],
                audience: _configuration["Tokens:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet]
        [Authorize(Roles = "PowerfulUser,KamAdmin,KamAdCoordinator")]
        public async Task<IActionResult> DetailRegister(int? id)
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
            var ListAsync = await _userHelper.GetAllKamsRecordsAsync();

            var model = ListAsync.Where(x => x.KamId == id).FirstOrDefault();
            return View(model);
        }

        [Authorize(Roles = "PowerfulUser,KamAdmin,KamCoordinator")]
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
                var kam = await KamsExists(id.Value);

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
                _flashMessage.Confirmation("The Trademars was deleted.");
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"The Products can't be deleted because it has related records. {ex.Message}");
            }
            return RedirectToAction(nameof(Index));
        }
        private async Task<Kams> KamsExists(int id)
        {
            var _kam = await _dataContext.Kams.FindAsync(id);
            return _kam;
        }
        public IActionResult NotAuthorized()
        {
            return View();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Common.Enums;
using OnlineOrderCart.Common.Responses;
using OnlineOrderCart.Web.DataBase;
using OnlineOrderCart.Web.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        private readonly DataContext _dataContext;
        private readonly IConfiguration _configuration;
       

        public ConverterHelper(DataContext dataContext, IConfiguration configuration)
        {
            _dataContext = dataContext;
            _configuration = configuration;
        }

        public Response<object> CreateSHA256(string Pass)
        {
            try
            {
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    byte[] sourceBytes = Encoding.UTF8.GetBytes(Pass);
                    byte[] hashBytes = sha256Hash.ComputeHash(sourceBytes);
                    string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);

                    return new Response<object>
                    {
                        IsSuccess = true,
                        Message = hash,
                    };
                }
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
        public Products ToProductEntity(AddProductViewModel model, bool isNew)
        {
            Guid name = Guid.NewGuid();
            return new Products
            {
                ProductId = isNew ? 0 : model.ProductId,
                Description = model.Description,
                CodeKey = model.CodeKey,
                ProductTypeId = model.ProductTypeId,
                ValueWithOutTax = model.ValueWithOutTax,
                TrademarkId = model.TrademarkId,
                Price = model.Price,
                UnitsInStock = model.UnitsInStock,
                ActivationFormId = model.ActivationFormId,
                ActivationTypeId = model.ActivationTypeId,
                SimTypeId = model.SimTypeId,
                OraclepId = model.OraclepId,
                IsDeleted = 0,
                RegistrationDate = DateTime.Now.ToUniversalTime(),
            };
        }
        public AddProductViewModel ToProductViewModel(Products model)
        {
            return new AddProductViewModel
            {
                ProductId = model.ProductId,
                CodeKey = model.CodeKey,
                Description = model.Description,
                ValueWithOutTax = model.ValueWithOutTax,
                OraclepId = model.OraclepId,
                TrademarkId = model.TrademarkId,
                ProductTypeId = model.ProductTypeId,
                ActivationFormId = model.ActivationFormId,
                ActivationTypeId = model.ActivationTypeId,
                SimTypeId = model.SimTypeId,
                Price = model.Price,
                UnitsInStock = model.UnitsInStock,
                IsDeleted = 0,
                RegistrationDate = DateTime.Now.ToUniversalTime(),
            };
        }
        public Users ToRegisterAsync(AddUserViewModel model, Guid imageId, bool isNew)
        {
            var _password = CreateSHA256(model.Password);
            if (_password.IsSuccess) {
                return new Users
                {
                    UserId = isNew ? 0 : model.UserId,
                    FirstName = model.FirstName,
                    LastName1 = model.LastName1,
                    LastName2 = model.LastName2,
                    GenderId = model.GenderId.ToString(),
                    Email = model.Email,
                    UserName = $"E{model.EmployeeNumber}",
                    ImageId = imageId,
                    PicturePath = string.IsNullOrEmpty(model.PicturePath) ? null : model.PicturePath,
                    Password = _password.Message,
                    IsDeleted = 100,
                    IsDistributor = 0,
                    RegistrationDate = DateTime.Now.ToUniversalTime(),
                };
            }
            else
            {
                return null;
            }
        }
        public async Task<Users> ToSRegisterAsync(AddUserViewModel model, Guid imageId, bool isNew)
        {
            var _Result = await _dataContext
                .Users
                .Where(u => u.UserId == model.UserId && u.UserName == model.Username)
                .FirstOrDefaultAsync();
                return new Users
                {
                    UserId = isNew ? 0 : model.UserId,
                    FirstName = model.FirstName,
                    LastName1 = model.LastName1,
                    LastName2 = model.LastName2,
                    GenderId = model.GenderId.ToString(),
                    Email = model.Email,
                    UserName = isNew ? $"E{model.EmployeeNumber}": model.Username,
                    ImageId = imageId,
                    Password = _Result.Password,
                    IsDeleted = isNew ? 100 : 0,
                    RegistrationDate = DateTime.Now.ToUniversalTime(),
                };
        }
        public Genders ToGenderStatus(int GenderStatusId)
        {
            switch (GenderStatusId)
            {
                case 1: return Genders.Femenino;
                case 2: return Genders.Masculino;
                default: return Genders.Generico;
            }
        }
        #region MyDistributor
        public async Task<Users> ToDistributorAsync(AddDistributorViewModel model, Guid imageId, bool isNew)
        {
            var _password = CreateSHA256(model.Password);
            if (_password.IsSuccess)
            {
                return new Users
                {
                    UserId = isNew ? 0 : model.UserId,
                    FirstName = model.FirstName,
                    LastName1 = model.LastName1,
                    LastName2 = model.LastName2,
                    GenderId = model.GenderId.ToString(),
                    Email = model.Email,
                    UserName = $"D{model.Debtor.ToString()}",
                    ImageId = imageId,
                    PicturePath = model.PicturePath,
                    Password = _password.Message,
                    IsDistributor = 1,
                    IsDeleted = 100,
                    RegistrationDate = DateTime.Now.ToUniversalTime(),
                };
            }
            else
            {
                return null;
            }
        }

        public Task<AddDistributorViewModel> ToDistributorViewModel(Users model)
        {
            throw new NotImplementedException();
        }

        public TokenResponse GetToken(string Username)
        {
            Claim[] claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, Username),
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

        public Warehouses ToWarehousetEntity(IndexWarehouseDistViewModel model, bool isNew)
        {
            return new Warehouses
            {
                StoreId = isNew ? 0 : model.StoreId,
                ShippingBranchNo = model.ShippingBranchNo,
                ShippingBranchName = model.ShippingBranchName,
                SapDescription = model.SapDescription,
                SapClient = model.SapClient,
                DistributorId = model.DistributorId,
                DelegationMunicipality = model.DelegationMunicipality,
                PostalCode = model.PostalCode,
                State = model.State,
                StreetNumber = model.StreetNumber,
                Suburd = model.Suburd,
                Warehousepvs = model.Warehousepvs,
                SimTypeId = model.SimTypeId,
                IsDeleted = 0,
                RegistrationDate = DateTime.Now.ToUniversalTime(),
            };
        }

        public async Task<PrOrderDetailTmps> ToOrdersTmpEntity(AddItemViewModel model, bool isNew)
        {
            var _pprice = await _dataContext.DeatilWarehouses
                    .Include(dw => dw.Products)
                    .Where(dw => dw.DeatilStoreId == model.DeatilStoreId).FirstOrDefaultAsync();

            return new PrOrderDetailTmps {
                Debtor = model.Debtor.ToString(),
                Quantity = model.Quantity,
                Observations = model.Observations,
                DeatilStoreId = model.DeatilStoreId,
                Price = _pprice.Products.Price,
                TaxRate = _pprice.Products.ValueWithOutTax,
                TypeofPaymentId = model.TypeofPaymentId,
                OrderStatus = "0",
                OrderDetailTmpId = isNew ? 0 : model.OrderDetailTmpId,
            };
        }
        #endregion
    }
}

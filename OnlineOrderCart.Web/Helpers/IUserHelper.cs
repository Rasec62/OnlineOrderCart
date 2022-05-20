using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Common.Responses;
using OnlineOrderCart.Web.Models;
namespace OnlineOrderCart.Web.Helpers
{
    public interface IUserHelper
    {
        Task<Response<RUKDViewModel>> GetValidateLoginAsync(LoginViewModel model);
        Task<UserManagerEntity> GetUserAsync(string email);
        Task<Response<UserManagerEntity>> GetUserByEmailAsync(string email);
        Task<Response<UserManagerEntity>> ChangePasswordAsync(UserManagerEntity managerEntity, string OldPassword, string NewPassword);
        Task<Response<UserManagerEntity>> ValidatePasswordAsync(UserManagerEntity user, string Password);
        Task<ObservableCollection<UserManagerEntity>> GetAllKamsRecordsAsync();
        Task<Response<Users>> AddUserAsync(AddUserViewModel model, Guid imageId);
        Task<Response<Users>> GetUserByIdAsync(string id);
        Task<Response<Users>> GetUserIdByUsernameAsync(string username , string id);
        Task<Response<UserManagerEntity>> GetKamByIdAsync(long id);
        Task<Response<object>> ConfirmEmailAsync(Users user, string Jwt, string token);
        Task<Response<object>> GetConfirmPasswordAsync(ConfirmPasswordViewModel model);
        Task<Response<object>> GetAllAvatarConfirmAsync(string UserName);
        Task<ObservableCollection<UserManagerEntity>> GetAllCoordRecordsAsync();
        Task<Response<UserManagerEntity>> GetCoordByIdAsync(long id);
        Task<Response<AddUserViewModel>> GetCoordByEmailAsync(string email);
        Task<Response<object>> ResetPasswordAsync(UserManagerEntity model, string jwt, string token, string password);
        Task<Response<TokenResponse>> GeneratePasswordResetTokenAsync(UserManagerEntity user);
        Task<Response<IndexKamCoordViewModel>> GetKamAdCoordinatorBySentIdAsync(long id);
        Task<Response<IndexKamCoordViewModel>> GetKamAdCoordinatorByActiveIdAsync(long id);
        Task<Response<object>> PutKamAdCoordByActiveIdAsync(IndexKamCoordViewModel modol);
    }
}

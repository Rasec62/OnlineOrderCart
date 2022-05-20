using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OnlineOrderCart.Web.Models;
using System.Linq;
using OnlineOrderCart.Web.DataBase;
using System.Data;
using OnlineOrderCart.Common.Responses;
using System.Threading.Tasks;
using OnlineOrderCart.Common.Entities;

namespace OnlineOrderCart.Web.Helpers
{
    public class MovementsHelper : IMovementsHelper
    {
        private readonly IConfiguration _configuration;
        private readonly ICombosHelper _combosHelper;
        private readonly DataContext _dataContext;
        private readonly IConverterHelper _converterHelper;
        List<SelectListItem> GetListKam;
        List<IndexUserDistEntity> GetListIndexUserDist;
        List<IndexKamCoordViewModel> GetListIndexUserKC;
        public MovementsHelper(IConfiguration configuration,
            ICombosHelper combosHelper, DataContext dataContext, IConverterHelper converterHelper)
        {
            _configuration = configuration;
            _combosHelper = combosHelper;
            _dataContext = dataContext;
            _converterHelper = converterHelper;
        }

        public IEnumerable<SelectListItem> GetComboAllKams()
        {
            try
            {
                string[] marks = new string[2] { "KD", "K" };

                var list = (from gr in _dataContext.RoleGroups
                            join r in _dataContext.Roles
                            on gr.RolId equals r.RolId
                            join u in _dataContext.Users
                            on gr.UserId equals u.UserId
                            join k in _dataContext.Kams
                            on u.UserId equals k.KamId
                            where (u.IsDeleted == 0 && marks.Contains(r.RolName) && k.IsCoordinator == 0 && k.EmployeeNumber != "911")
                            select new SelectListItem
                            {
                                Text = $"{u.FirstName}{" "}{u.LastName1}{" "}{u.LastName2}",
                                Value = $"{k.KamId}"
                            }).OrderBy(x => x.Text).ToList();

                list.Insert(0, new SelectListItem
                {
                    Text = "(Select a Kam...)",
                    Value = "0"
                });

                return list;
            }
            catch (Exception)
            {
                return null;
            }

        }
        public IEnumerable<SelectListItem> GetSqlDataKams()
        {
            GetListKam = new List<SelectListItem>();
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"))) {
                try
                {
                    string query = $"{"Select k.KamId as 'KamManagerId', u.FirstName+' '+ u.LastName1+' '+ u.LastName2 as 'FullName'  from dbo.RoleGroups rg Inner Join dbo.Roles r On rg.RolId = r.RolId Inner Join dbo.Users u On rg.UserId = u.UserId  Inner join dbo.Kams k on u.UserId = k.UserId  Where k.CodeKey in('JAAA', 'JAF') and k.IsDeleted = 0"}";
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.Connection = connection;
                        connection.Open();
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                GetListKam.Add(new SelectListItem
                                {
                                    Value = sdr["KamManagerId"].ToString().ToUpper(),
                                    Text = sdr["FullName"].ToString().ToUpper(),
                                });
                            }
                        }
                    }
                    GetListKam.Insert(0, new SelectListItem
                    {
                        Text = "(Select a Kam...)",
                        Value = "0"
                    });
                    return GetListKam;
                }
                catch (System.Exception)
                {

                    return null;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        public IEnumerable<IndexUserDistEntity> GetSqlAllDataDistributors() {
            GetListIndexUserDist = new List<IndexUserDistEntity>();
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"))) {
                try
                {
                    //string query = $"{" Select u.UserId,ka.KamId, u.PicturePath, u.Email,ka.EmployeeNumber,u.FirstName,u.LastName1,u.LastName2, u.FirstName +''+ u.LastName1+' '+u.LastName2 as 'KFullName', u.GenderId ,"}{"Case u.GenderId When 1 then 'Femenino'when 2 then 'Masculina' else 'Generico' end as'Gender',"}{" 'PicturePath', u.ImageId, u.Username, r.RolName,r.RolId, d.BusinessName,d.Debtor,d.DistributorId, d.IsDeleted"}{" from dbo.Users u With(Nolock)  Inner Join dbo.Distributors d With(Nolock) On u.UserId = d.UserId Inner Join dbo.Kams ka With(Nolock) On ka.KamId = d.KamId Inner Join dbo.RoleGroups rg with(Nolock) On rg.UserId = u.UserId Inner Join dbo.Roles r With(Nolock) On rg.RolId = r.RolId Where u.IsDeleted = 0 and u.IsDistributor = 1 and d.IsDeleted = 0"}";
                    string query = $"{" Select u.UserId,ka.KamId, u.PicturePath, u.Email,ka.EmployeeNumber,u.FirstName,u.LastName1,u.LastName2, u.FirstName +''+ u.LastName1+' '+u.LastName2 as 'KFullName', u.GenderId ,"}{"Case u.GenderId When 1 then 'Femenino'when 2 then 'Masculina' else 'Generico' end as'Gender',"}{" 'PicturePath', u.ImageId, u.Username, r.RolName,r.RolId, d.BusinessName,d.Debtor,d.DistributorId, d.IsDeleted, dbo.CountWarehousesDist(d.DistributorId) as 'CountW'"}{" from dbo.Users u With(Nolock)  Inner Join dbo.Distributors d With(Nolock) On u.UserId = d.UserId Inner Join dbo.Kams ka With(Nolock) On ka.KamId = d.KamId Inner Join dbo.RoleGroups rg with(Nolock) On rg.UserId = u.UserId Inner Join dbo.Roles r With(Nolock) On rg.RolId = r.RolId Where u.IsDeleted in(0,1,100) and u.IsDistributor = 1 and d.IsDeleted in(0,1,100)"}";
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        
                        cmd.Connection = connection;
                        connection.Open();
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                GetListIndexUserDist.Add(new IndexUserDistEntity
                                {
                                    UserId = Convert.ToInt32(sdr["UserId"].ToString()),
                                    KamId = Convert.ToInt32(sdr["KamId"].ToString()),
                                    Email = sdr["Email"].ToString(),
                                    EmployeeNumber = sdr["EmployeeNumber"].ToString(),
                                    FirstName = sdr["FirstName"].ToString(),
                                    LastName1 = sdr["LastName1"].ToString(),
                                    LastName2 = sdr["LastName2"].ToString(),
                                    GenderId = Convert.ToInt32(sdr["GenderId"].ToString()),
                                    Gender = sdr["RolName"].ToString(),
                                    ImageId = (Guid)sdr["ImageId"],
                                    PicturePath = string.IsNullOrEmpty(sdr["ImageId"].ToString()) ? $"https://localhost:44366/Image/noimage.png" : $"https://onsalelayout.blob.core.windows.net/users/{(Guid)sdr["ImageId"]}",
                                    PicturefullPath = string.IsNullOrEmpty(sdr["PicturePath"].ToString()) ? $"https://localhost:44366/Image/noimage.png" : sdr["PicturePath"].ToString(),
                                    Username = sdr["UserName"].ToString(),
                                    RolName = sdr["RolName"].ToString().ToUpper(),
                                    KFullName = GetSqlAllDataKams(Convert.ToInt32(sdr["KamId"].ToString())),
                                    Debtor = sdr["Debtor"].ToString(),
                                    BusinessName = sdr["BusinessName"].ToString().ToUpper(),
                                    DistributorId = Convert.ToInt32(sdr["DistributorId"].ToString()),
                                    IsActive = Convert.ToInt32(sdr["IsDeleted"].ToString()) == 0 ? true : false,
                                    CountW = Convert.ToInt32(sdr["CountW"].ToString()),
                                }); 
                            }
                        }
                    }
                    return GetListIndexUserDist;
                }
                catch (System.Exception)
                {

                    return null;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public string GetSqlAllDataKams(long KamId)
        {
            string _Result = string.Empty;
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"))) {
                try
                {
                    string query = $"{"Select u.FirstName +' '+ u.LastName1 +' '+ u.LastName2 as 'FullName' from dbo.Users u With(Nolock) Inner Join dbo.Kams k With(Nolock) On u.UserId = k.UserId Where u.IsDeleted = 0 and k.IsDeleted = 0 and k.KamId = @KamId"}";
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.Parameters.Add("@KamId", SqlDbType.BigInt);
                        cmd.Parameters["@KamId"].Value = KamId;
                        
                        cmd.Connection = connection;
                        connection.Open();
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {


                                _Result = sdr["FullName"].ToString().ToUpper();
                                
                            }
                        }
                    }
                   
                    return _Result;
                }
                catch (System.Exception)
                {

                    return null;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        public async Task<Response<RolAvatarConfirm>> GetRolAvatarConfirmAsync(string UserName)
        {
            try
            {

                var _Object = await (from g in _dataContext.RoleGroups
                                     join r in _dataContext.Roles on g.RolId equals r.RolId
                                     join u in _dataContext.Users on g.UserId equals u.UserId
                                     where u.UserName.Equals(UserName)
                                     select new RolAvatarConfirm
                                     {
                                         RolId = r.RolId,
                                         RolName = r.RolName,
                                         UserId = u.UserId,
                                         UserName = u.UserName,
                                     }).FirstOrDefaultAsync();

                return new Response<RolAvatarConfirm>
                {
                    IsSuccess = true,
                    Result = _Object,
                };
            }
            catch (Exception ex)
            {
                return new Response<RolAvatarConfirm>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

        public IEnumerable<SelectListItem> GetSqlComboAllKams()
        {
            var GetListKam = new List<SelectListItem>();
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                try
                {
                    string query = $"{"Select u.FirstName +' '+ u.LastName1 +' '+ u.LastName2 'FullKam', k.KamId as 'KamManagerId' from dbo.RoleGroups rg with(Nolock) "}{"Inner Join dbo.Roles r With(Nolock) On rg.RolId = r.RolId Inner Join dbo.Users u With(Nolock) On rg.UserId = u.UserId Inner Join dbo.Kams k With(Nolock) On u.UserId = k.UserId "}{" Where r.CodeKey In('KD','K') and (r.IsDeleted = 0 or k.IsDeleted = 0) and  u.IsDeleted = 0 and k.IsDeleted = 0 and k.IsCoordinator=0"}";
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.Connection = connection;
                        connection.Open();
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                GetListKam.Add(new SelectListItem
                                {
                                    Value = sdr["KamManagerId"].ToString(),
                                    Text = sdr["FullKam"].ToString().ToUpper(),
                                });
                            }
                        }
                    }
                    GetListKam.Insert(0, new SelectListItem
                    {
                        Text = "(Select a Kam...)",
                        Value = "0"
                    });
                    return GetListKam;
                }
                catch (SqlException sqlEx)
                {
                    string msg = "Insert/Update Error:";
                    msg += sqlEx.Message;
                    throw new Exception(msg);
                }
                catch (System.Exception)
                {

                    return null;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        public List<IndexUserDistEntity> GetSqlforwardingDataDistributors()
        {
            GetListIndexUserDist = new List<IndexUserDistEntity>();
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                try
                {
                    string query = $"{" Select u.UserId,ka.KamId, u.PicturePath, u.Email,ka.EmployeeNumber,u.FirstName,u.LastName1,u.LastName2, u.FirstName +''+ u.LastName1+' '+u.LastName2 as 'KFullName', u.GenderId ,"}{"Case u.GenderId When 1 then 'Femenino'when 2 then 'Masculina' else 'Generico' end as'Gender',"}{" 'PicturePath', u.ImageId, u.Username, r.RolName,r.RolId, d.BusinessName,d.Debtor,d.DistributorId"}{" from dbo.Users u With(Nolock)  Inner Join dbo.Distributors d With(Nolock) On u.UserId = d.UserId Inner Join dbo.Kams ka With(Nolock) On ka.KamId = d.KamId Inner Join dbo.RoleGroups rg with(Nolock) On rg.UserId = u.UserId Inner Join dbo.Roles r With(Nolock) On rg.RolId = r.RolId Where u.IsDeleted = 100 and (u.IsDistributor = 1 or d.IsDeleted = 100)"}";
                    using (SqlCommand cmd = new SqlCommand(query))
                    {

                        cmd.Connection = connection;
                        connection.Open();
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                GetListIndexUserDist.Add(new IndexUserDistEntity
                                {
                                    UserId = Convert.ToInt32(sdr["UserId"].ToString()),
                                    KamId = Convert.ToInt32(sdr["KamId"].ToString()),
                                    Email = sdr["Email"].ToString(),
                                    EmployeeNumber = sdr["EmployeeNumber"].ToString(),
                                    FirstName = sdr["FirstName"].ToString(),
                                    LastName1 = sdr["LastName1"].ToString(),
                                    LastName2 = sdr["LastName2"].ToString(),
                                    GenderId = Convert.ToInt32(sdr["GenderId"].ToString()),
                                    Gender = sdr["RolName"].ToString(),
                                    ImageId = (Guid)sdr["ImageId"],
                                    PicturePath = string.IsNullOrEmpty(sdr["ImageId"].ToString()) ? $"https://localhost:44366/Image/noimage.png" : $"https://onsalelayout.blob.core.windows.net/users/{(Guid)sdr["ImageId"]}",
                                    PicturefullPath = string.IsNullOrEmpty(sdr["PicturePath"].ToString()) ? $"https://localhost:44366/Image/noimage.png" : sdr["PicturePath"].ToString(),
                                    Username = sdr["UserName"].ToString(),
                                    RolName = sdr["RolName"].ToString().ToUpper(),
                                    KFullName = GetSqlAllDataKams(Convert.ToInt32(sdr["KamId"].ToString())),
                                    Debtor = sdr["Debtor"].ToString(),
                                    BusinessName = sdr["BusinessName"].ToString().ToUpper(),
                                    DistributorId = Convert.ToInt32(sdr["DistributorId"].ToString()),
                                });
                            }
                        }
                    }
                    return GetListIndexUserDist;
                }
                catch (System.Exception)
                {

                    return null;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task<Response<UserActivations>> GetSqlEmailforwardingDataDistributors(long UserId, string UserName, string Email)
        {
            using (var transaction = _dataContext.Database.BeginTransaction())
            {
                try
                {
                    var _UserActivation = await _dataContext
                        .UserActivations
                        .Where(a => a.UserId.Equals(UserId) && a.UserName == UserName)
                        .FirstOrDefaultAsync();
                    if (_UserActivation == null)
                    {
                        return new Response<UserActivations>{
                            IsSuccess = false,
                            Message = "Do not Data",
                        };
                    }
                    _dataContext.UserActivations.Remove(_UserActivation);
                    await _dataContext.SaveChangesAsync();

                    TokenResponse myToken = _converterHelper.GetToken(UserName);

                    Guid activationCode = Guid.NewGuid();
                    var userActivations = new UserActivations
                    {
                        ActivationCode = activationCode,
                        UserId = UserId,
                        UserName = UserName,
                        EventAction = "forwarding - Email - Avatar",
                        JwtId = myToken.Token,
                        CreationDate = DateTime.UtcNow.ToUniversalTime(),
                        ExpiryDate = myToken.Expiration,
                        IsDeleted = 0,
                        RegistrationDate = DateTime.Now.ToUniversalTime(),
                    };

                    _dataContext.UserActivations.Add(userActivations);
                    await _dataContext.SaveChangesAsync();

                    transaction.Commit();
                    return new Response<UserActivations>
                    {
                        IsSuccess = true,
                        Message = "Win",
                        Result = userActivations,
                    };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    string message = string.Format("<b>Message:</b> {0}<br /><br />", ex.Message);
                    message += string.Format("<b>StackTrace:</b> {0}<br /><br />", ex.StackTrace.Replace(Environment.NewLine, string.Empty));
                    message += string.Format("<b>Source:</b> {0}<br /><br />", ex.Source.Replace(Environment.NewLine, string.Empty));
                    message += string.Format("<b>TargetSite:</b> {0}", ex.TargetSite.ToString().Replace(Environment.NewLine, string.Empty));
                    return new Response<UserActivations>
                    {
                        IsSuccess = false,
                        Message = message,
                    };
                }
            }

        }
        public async Task<List<TblOptionalEmail>> GetDetailsOptionalEmailAsync(long UserId, string Debtor)
        {
            try
            {
                List<TblOptionalEmail> Optionemail = await _dataContext.TblOptionalEmail
                                .Where(x => x.UserId == UserId && x.Debtor.Equals(Debtor))
                                .OrderBy(x => x.Id)
                                .ToListAsync();
                return Optionemail;
            }
            catch (Exception){
                return null;
            }
        }

        public async Task<List<IndexKamCoordViewModel>> GetSqlforwardingDataKamAdCoords()
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"))){
                GetListIndexUserKC = new List<IndexKamCoordViewModel>();
                try{
                    string query = $"{" Select  u.UserId 'Id', k.KamId 'kcId', k.EmployeeNumber 'NoEmployee' , u.Email, u.UserName 'KickName',u.FirstName +' '+ u.LastName1+' ' + u.LastName2 as 'FullName',u.PicturePath 'Path', k.IsCoordinator "}{" from dbo.Users u with(Nolock) Inner Join dbo.Kams k with(Nolock) on u.UserId = k.UserId "}{" Where u.IsDeleted Not In(0,1)  and k.IsDeleted Not In(0,1) and u.IsDistributor = 0 "}";
                    using (SqlCommand cmd = new SqlCommand(query)) {
                        cmd.Connection = connection;
                       await connection.OpenAsync();
                        using (SqlDataReader sdr = cmd.ExecuteReader()) {
                            while (sdr.Read()){
                                GetListIndexUserKC.Add(new IndexKamCoordViewModel{
                                    Id = Convert.ToInt32(sdr["Id"].ToString()),
                                    KcId = Convert.ToInt64(sdr["KcId"].ToString()),
                                    Email = sdr["Email"].ToString(),
                                    NoEmployee = sdr["NoEmployee"].ToString(),
                                    FullName = sdr["FullName"].ToString(),
                                    Path = sdr["Path"].ToString(),
                                    KickName = sdr["KickName"].ToString(),
                                    IsCoordinator = Convert.ToInt32(sdr["IsCoordinator"].ToString()),
                                    IsKam = Convert.ToInt32(sdr["IsCoordinator"].ToString())== 0 ? true: false, 
                                });
                            }
                        }
                    }
                    return GetListIndexUserKC.OrderBy(u => u.KcId).ToList();
                }
                catch (Exception){
                    return null;
                }
                finally{
                   await connection.CloseAsync();
                }
            }
        }
    }
}

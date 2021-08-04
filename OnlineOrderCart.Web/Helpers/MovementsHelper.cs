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

namespace OnlineOrderCart.Web.Helpers
{
    public class MovementsHelper : IMovementsHelper
    {
        private readonly IConfiguration _configuration;
        private readonly ICombosHelper _combosHelper;
        List<SelectListItem> GetListKam;
        List<IndexUserDistEntity> GetListIndexUserDist;
        public MovementsHelper(IConfiguration configuration, ICombosHelper combosHelper, DataContext dataContext)
        {
            _configuration = configuration;
            _combosHelper = combosHelper;
        }

        public IEnumerable<SelectListItem> GetSqlComboAllKams()
        {
            var GetListKam = new List<SelectListItem>();
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"))) {
                try {
                    string query = $"{"Select u.FirstName +' '+ u.LastName1 +' '+ u.LastName2 'FullKam', k.KamId from dbo.RoleGroups rg with(Nolock) "}{"Inner Join dbo.Roles r With(Nolock) On rg.RolId = r.RolId Inner Join dbo.Users u With(Nolock) On rg.UserId = u.UserId Inner Join dbo.Kams k With(Nolock) On u.UserId = k.UserId "}{" Where r.CodeKey In('KD','K') and (r.IsDeleted = 0 or k.IsDeleted = 0)"}";
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
                                    Value = sdr["KamId"].ToString(),
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
                catch (SqlException sqlEx) {
                    string msg = "Insert/Update Error:";
                    msg += sqlEx.Message;
                    throw new Exception(msg);
                }
                catch (System.Exception) {

                    return null;
                }
                finally {
                    connection.Close();
                }
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
                    string query = $"{" Select u.UserId,ka.KamId, u.PicturePath, u.Email,ka.EmployeeNumber,u.FirstName,u.LastName1,u.LastName2, u.FirstName +''+ u.LastName1+' '+u.LastName2 as 'KFullName', u.GenderId ,"}{"Case u.GenderId When 1 then 'Femenino'when 2 then 'Masculina' else 'Generico' end as'Gender',"}{" 'PicturePath', u.ImageId, u.Username, r.RolName,r.RolId, d.BusinessName,d.Debtor,d.DistributorId"}{" from dbo.Users u With(Nolock)  Inner Join dbo.Distributors d With(Nolock) On u.UserId = d.UserId Inner Join dbo.Kams ka With(Nolock) On ka.KamId = d.KamId Inner Join dbo.RoleGroups rg with(Nolock) On rg.UserId = u.UserId Inner Join dbo.Roles r With(Nolock) On rg.RolId = r.RolId Where u.IsDeleted = 0 and u.IsDistributor = 1 or d.IsDeleted = 0"}";
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

        public string GetSqlAllDataKams(long KamId)
        {
            string _Result = string.Empty;
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"))) {
                try
                {
                    string query = $"{"Select u.FirstName +' '+ u.LastName1 +' '+ u.LastName2 as 'FullName' from dbo.Users u With(Nolock) Inner Join dbo.Kams k With(Nolock) On u.UserId = k.KamId Where u.IsDeleted = 0 and k.IsDeleted = 0 and k.KamId = @KamId"}";
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
    }
}

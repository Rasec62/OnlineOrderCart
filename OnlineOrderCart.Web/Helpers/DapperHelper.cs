using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OnlineOrderCart.Common.Responses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.Helpers
{
    public class DapperHelper: IDapperHelper
    {
        private readonly IConfiguration _config;
        private string Connectionstring = "DefaultConnection";
        public DapperHelper(IConfiguration config)
        {
            _config = config;
        }

        public void Dispose()
        {}

        public int Execute(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            throw new System.NotImplementedException();
        }
        public T Get<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.Text)
        {
            using IDbConnection db = new SqlConnection(_config.GetConnectionString(Connectionstring));
            return db.Query<T>(sp, parms, commandType: commandType).FirstOrDefault();
        }

        public List<T> GetAll<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            using IDbConnection db = new SqlConnection(_config.GetConnectionString(Connectionstring));
            return db.Query<T>(sp, parms, commandType: commandType).ToList();
        }

        public async Task<GenericResponse<T>> GetAllAsync<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.Text)
        {
            
            using IDbConnection db = new SqlConnection(_config.GetConnectionString(Connectionstring));
            try
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                var list = await db.QueryAsync<T>(sp, parms, commandType: commandType);
                return new GenericResponse<T>
                {
                    IsSuccess = true,
                    ListResults = list.ToList(),
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<T>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
            finally
            {
                if (db.State == ConnectionState.Open)
                    db.Close();
            }   
        }
        public DbConnection GetDbconnection()
        {
            return new SqlConnection(_config.GetConnectionString(Connectionstring));
        }

        public Response<T> GetOnly<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            T result;
            using IDbConnection db = new SqlConnection(_config.GetConnectionString(Connectionstring));
            try
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                result = db.Query<T>(sp, parms, commandType: commandType).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return new Response<T>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
            finally
            {
                if (db.State == ConnectionState.Open)
                    db.Close();
            }
            return new Response<T>
            {
                IsSuccess = true,
                Result = result,
            };
        }

        public Response<T> GetOnlyAvatar<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.Text)
        {
            T result;
            using IDbConnection db = new SqlConnection(_config.GetConnectionString(Connectionstring));
            try
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                result = db.Query<T>(sp, parms, commandType: commandType).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return new Response<T>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
            finally
            {
                if (db.State == ConnectionState.Open)
                    db.Close();
            }
            return new Response<T>
            {
                IsSuccess = true,
                Result = result,
            };
        }

        public Response<T> Insert<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            T result;
            using IDbConnection db = new SqlConnection(_config.GetConnectionString(Connectionstring));
            try
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                using var tran = db.BeginTransaction();
                try
                {
                    result = db.Query<T>(sp, parms, commandType: commandType, transaction: tran).FirstOrDefault();
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    return new Response<T> {
                        IsSuccess = false,
                        Message = ex.Message,
                    };
                }
            }
            catch (Exception ex)
            {
                return new Response<T>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
            finally
            {
                if (db.State == ConnectionState.Open)
                    db.Close();
            }
            return new Response<T>
            {
                IsSuccess = true,
                Result = result,
            };
        }
        public Response<T> Update<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            T result;
            using IDbConnection db = new SqlConnection(_config.GetConnectionString(Connectionstring));
            try
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                using var tran = db.BeginTransaction();
                try
                {
                    result = db.Query<T>(sp, parms, commandType: commandType, transaction: tran).FirstOrDefault();
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    return new Response<T>
                    {
                        IsSuccess = false,
                        Message = ex.Message,
                    };
                }
            }
            catch (Exception ex)
            {
                return new Response<T>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
            finally
            {
                if (db.State == ConnectionState.Open)
                    db.Close();
            }

            return new Response<T>
            {
                IsSuccess = true,
                Result = result,
            };
        }
    }
}

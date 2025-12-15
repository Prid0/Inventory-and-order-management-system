using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Pim.Data;
using System.Data;

namespace Pim.Utility.SqlHelper
{
    public class ExecuteSp
    {
        private readonly ApplicationDbContext _dbContext;

        public ExecuteSp(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<T>> ExecuteStoredProcedureListAsync<T>(
            string storedProcedure,
            params SqlParameter[] sqlParams)
        {
            var connection = _dbContext.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            var dapperParams = ConvertToDynamicParameters(sqlParams);

            var rows = await connection.QueryAsync<T>(
                storedProcedure,
                dapperParams,
                commandType: CommandType.StoredProcedure
            );

            return rows.AsList();
        }

        public async Task<T> ExecuteStoredProcedureAsync<T>(
           string storedProcedure,
           params SqlParameter[] sqlParams)
        {
            var connection = _dbContext.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            var dapperParams = ConvertToDynamicParameters(sqlParams);

            var rows = await connection.QueryAsync<T>(
                storedProcedure,
                dapperParams,
                commandType: CommandType.StoredProcedure
            );

            return rows.FirstOrDefault();
        }

        private DynamicParameters ConvertToDynamicParameters(SqlParameter[] sqlParams)
        {
            var dp = new DynamicParameters();

            foreach (var p in sqlParams)
            {
                dp.Add(
                    name: p.ParameterName,
                    value: p.Value == DBNull.Value ? null : p.Value,
                    dbType: (DbType?)p.SqlDbType,
                    direction: p.Direction
                );
            }

            return dp;
        }
    }
}



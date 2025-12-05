using Microsoft.EntityFrameworkCore;
using Pim.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Threading.Tasks;

namespace Pim.Helper.SqlHelper
{
    public class ExecuteStoredProcedure
    {
        private readonly ApplicationDbContext _dbContext;

        public ExecuteStoredProcedure(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<TResult>> ExecuteStoredProcedureListAsync<TResult>(
            string storedProcedure,
            params DbParameter[] parameters)
            where TResult : class, new()
        {
            var results = new List<TResult>();

            using (var connection = _dbContext.Database.GetDbConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = storedProcedure;
                command.CommandType = CommandType.StoredProcedure;

                // Add parameters
                foreach (var p in parameters)
                    command.Parameters.Add(p);

                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    var props = typeof(TResult).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                    while (await reader.ReadAsync())
                    {
                        TResult item = new TResult();

                        foreach (var prop in props)
                        {
                            if (!reader.HasColumn(prop.Name))
                                continue;

                            if (reader[prop.Name] == DBNull.Value)
                            {
                                prop.SetValue(item, null);
                                continue;
                            }

                            prop.SetValue(item, Convert.ChangeType(reader[prop.Name], prop.PropertyType));
                        }

                        results.Add(item);
                    }
                }
            }

            return results;
        }
    }

    // Extend DbDataReader to check column existence safely
    public static class DbDataReaderExtensions
    {
        public static bool HasColumn(this DbDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;

            return false;
        }
    }
}

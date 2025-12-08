using Microsoft.EntityFrameworkCore;
using Pim.Data;
using System.Data;
using System.Data.Common;

namespace Pim.Utility.SqlHelper
{
    public class ExecuteSp
    {
        private readonly ApplicationDbContext _dbContext;

        public ExecuteSp(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<TResult>> ExecuteStoredProcedureListAsync<TResult>(
            string storedProcedure,
            params DbParameter[] parameters)
            where TResult : class, new()
        {
            var results = new List<TResult>();

            await using var connection = _dbContext.Database.GetDbConnection();
            await using var command = connection.CreateCommand();

            command.CommandText = storedProcedure;
            command.CommandType = CommandType.StoredProcedure;

            foreach (var p in parameters)
                command.Parameters.Add(p);

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();

            var properties = typeof(TResult).GetProperties();

            while (await reader.ReadAsync())
            {
                var item = new TResult();

                foreach (var prop in properties)
                {
                    if (!reader.HasColumn(prop.Name))
                        continue;

                    var value = reader[prop.Name];
                    prop.SetValue(item, value == DBNull.Value ? null : value);
                }

                results.Add(item);
            }

            await reader.CloseAsync();

            await command.DisposeAsync();

            return results;
        }
    }

    public static class DbDataReaderExtensions
    {
        public static bool HasColumn(this DbDataReader reader, string name)
        {
            for (int i = 0; i < reader.FieldCount; i++)
                if (reader.GetName(i)
                    .Equals(name, StringComparison.OrdinalIgnoreCase))
                    return true;

            return false;
        }
    }
}

using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ToDoList.Infrastructure.Data;

public sealed class DbConnectionFactory(IConfiguration configuration) : IDbConnectionFactory
{
    public IDbConnection CreateConnection()
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException(
                                   "Connection string 'DefaultConnection' was not found.");

        return new SqlConnection(connectionString);
    }
}
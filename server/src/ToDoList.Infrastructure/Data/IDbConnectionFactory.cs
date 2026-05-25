using System.Data;

namespace ToDoList.Infrastructure.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
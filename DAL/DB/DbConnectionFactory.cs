using System.Data;
using System.Data.SqlClient;

namespace DAL.DB
{
    public sealed class DbConnectionFactory
    {
        private readonly string connectionString;
        public DbConnectionFactory(string connectionStr)
        {
            connectionString = connectionStr;
        }

        public IDbConnection CreateConnection() => new SqlConnection(connectionString);
    }

    

}

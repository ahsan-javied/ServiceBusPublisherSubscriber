using DAL.DB;
using Dapper;

namespace DAL.Repository
{
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private DbConnectionFactory dbConnectionFactory;

        public GenericRepository(DbConnectionFactory dbConn)
        {
            dbConnectionFactory = dbConn;
        }

        public async Task<IEnumerable<T>> GetAllByParms(string sql, T data)
        {
            using var dbConnection = dbConnectionFactory.CreateConnection();
            dbConnection.Open();
            //Note: Uncomment below line for testing only
            //await Task.Delay(TimeSpan.FromSeconds(30)); // Delay for 30 seconds
            return await dbConnection.QueryAsync<T>(sql, data);
        }

        public async Task<T> GetSingleByParms(string sql, T data)
        {
            using var dbConnection = dbConnectionFactory.CreateConnection();
            dbConnection.Open();
            return await dbConnection.QueryFirstOrDefaultAsync<T>(sql, new { data });
        }

        public Task Insert(string sql, T obj)
        {
            throw new NotImplementedException();
        }

        public async Task BulkUpdate(string sql, List<T> obj)
        {
            using var dbConnection = dbConnectionFactory.CreateConnection();
            dbConnection.Open();
            await dbConnection.ExecuteAsync(sql, param: obj);
        }

        public async Task Update(string sql, T obj)
        {
            using var dbConnection = dbConnectionFactory.CreateConnection();
            dbConnection.Open();
            await dbConnection.ExecuteAsync(sql, param: obj);
        }

        public Task Delete(string sql, T obj)
        {
            throw new NotImplementedException();
        }

        public Task Save()
        {
            throw new NotImplementedException();
        }

    }
}

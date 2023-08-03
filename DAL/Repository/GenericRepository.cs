using System.Data;
using Dapper;

namespace DAL.Repository
{
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private IDbConnection dbConnection;
        //private IDbConnection dbConnection
        //{
        //    get
        //    {
        //        return dbConnection ?? new SqlConnection(Configuration.GetConnectionString("MyConnectionString"));
        //    }
        //}

        public GenericRepository(IDbConnection dbConn)
        {
            dbConnection = dbConn;
        }

        public async Task<IEnumerable<T>> GetAllByParms(string sql, T data)
        {
            try
            {
                dbConnection.Open();
                return await dbConnection.QueryAsync<T>(sql, data);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                dbConnection.Close();
            }

        }

        public async Task<T> GetSingleByParms(string sql, T data)
        {
            try
            {
                dbConnection.Open();
                return await dbConnection.QueryFirstOrDefaultAsync<T>(sql, new { data });
            }
            catch (Exception)
            {
                throw;
            }
            finally
            { 
                dbConnection.Close(); 
            }
        }

        public Task Insert(string sql, T obj)
        {
            throw new NotImplementedException();
        }

        public async Task BulkUpdate(string sql, List<T> obj)
        {
            try
            {
                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, param: obj);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                dbConnection.Close();
            }
        }

        public async Task Update(string sql, T obj)
        {
            try
            {
                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, param: obj);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                dbConnection.Close();
            }
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

    //public class GenericRepository
    //{
    //    private readonly string _connectionString;

    //    public GenericRepository(string connectionString)
    //    {
    //        _connectionString = connectionString;
    //    }

    //    private IDbConnection Connection => new SqlConnection(_connectionString);

    //    #region Notification

    //    public IEnumerable<Notification> GetAllNotifications(Notification notification)
    //    {
    //        using IDbConnection dbConnection = Connection;

    //        try
    //        {
    //            dbConnection.Open();
    //            var data = dbConnection.Query<Notification>("SELECT * FROM Notifications where IsDelivered = @IsDelivered AND PickAndLock = @PickAndLock", notification);
    //            return data;
    //        }
    //        catch (Exception)
    //        {
    //            throw;
    //        }
    //        finally 
    //        {
    //            dbConnection.Close();
    //        }

    //    }

    //    public void UpdateNotification(Notification notification)
    //    {
    //        using IDbConnection dbConnection = Connection;

    //        try
    //        {
    //            dbConnection.Open();
    //            string sql = "UPDATE Notifications SET IsDelivered = @IsDelivered, DeliveredDT = @DeliveredDT, IsRead = @IsRead, ReadDT = @ReadDT WHERE Id = @Id";

    //            if (notification.IsDelivered)
    //                sql = "UPDATE Notifications SET IsDelivered = @IsDelivered, DeliveredDT = @DeliveredDT WHERE Id = @Id";

    //            if (notification.IsRead)
    //                sql = "UPDATE Notifications SET IsRead = @IsRead, ReadDT = @ReadDT WHERE Id = @Id";

    //            dbConnection.Execute(sql, notification);
    //        }
    //        catch (Exception)
    //        {
    //            throw;
    //        }
    //        finally
    //        {
    //            dbConnection.Close();
    //        }
    //    }

    //    public void BulkUpdateNotification(List<Notification> notifications)
    //    {
    //        using IDbConnection dbConnection = Connection;

    //        try
    //        {
    //            dbConnection.Open();
    //            string sql = "UPDATE Notifications SET " +
    //                "IsDelivered = @IsDelivered, DeliveredDT = @DeliveredDT, " +
    //                "IsRead = @IsRead, ReadDT = @ReadDT, " +
    //                "PickAndLock = @PickAndLock, PickAndLockDT= @PickAndLockDT " +
    //                "WHERE Id = @Id";

    //            dbConnection.Execute(sql, param: notifications);
    //        }
    //        catch (Exception)
    //        {
    //            throw;
    //        }
    //        finally
    //        {
    //            dbConnection.Close();
    //        }
    //    }

    //    #endregion

    //    public IEnumerable<UserMessage> GetAllUserMessages(UserMessage UserMessage)
    //    {
    //        using IDbConnection dbConnection = Connection;
    //        dbConnection.Open();
    //        var data = dbConnection.Query<UserMessage>("SELECT * FROM UserMessage where IsDelivered = @IsDelivered", UserMessage);
    //        dbConnection.Close();
    //        return data;
    //    }

    //    public UserMessage GetUserMessageById(int id)
    //    {
    //        using IDbConnection dbConnection = Connection;
    //        dbConnection.Open();
    //        var date = dbConnection.QueryFirstOrDefault<UserMessage>("SELECT * FROM UserMessage WHERE MessageId = @MessageId", new { MessageId = id });
    //        dbConnection.Close();
    //        return date;
    //    }

    //    public void InsertUserMessage(UserMessage UserMessage)
    //    {
    //        using IDbConnection dbConnection = Connection;
    //        dbConnection.Open();
    //        dbConnection.Execute("INSERT INTO UserMessage (SenderId, Content) VALUES (@SenderId, @Content)", UserMessage);
    //        dbConnection.Close();
    //    }

    //    public void UpdateUserMessage(UserMessage UserMessage)
    //    {
    //        using IDbConnection dbConnection = Connection;
    //        dbConnection.Open();
    //        dbConnection.Execute("UPDATE UserMessage SET IsDelivered = @IsDelivered, DeliveredDT = @DeliveredDT WHERE MessageId = @MessageId",
    //            UserMessage);
    //        dbConnection.Close();                        
    //    }

    //    public void DeleteUserMessage(int id)
    //    {
    //        using IDbConnection dbConnection = Connection;
    //        dbConnection.Open();
    //        dbConnection.Execute("DELETE FROM UserMessage WHERE MessageId = @MessageId", new { MessageId = id });
    //        dbConnection.Close();
    //    }
    //}
}

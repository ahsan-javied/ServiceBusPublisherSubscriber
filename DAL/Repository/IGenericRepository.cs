
namespace DAL.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllByParms(string sql, T data);
        Task<T> GetSingleByParms(string sql, T data);
        Task Insert(string sql, T obj);
        Task Update(string sql, T obj);
        Task BulkUpdate(string sql, List<T> obj);
        Task Delete(string sql, T obj);
        Task Save();
    }
}

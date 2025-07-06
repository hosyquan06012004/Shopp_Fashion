namespace BTL_Mongodb.Models.BusinessModels
{
    public interface IRepositoryGeneric<T,K>
    {
        bool Insert(T entity);
        bool Update(T entity);
        bool Delete(K id);
        T GetById(K id);
        List<T> GetAll();
    }
}

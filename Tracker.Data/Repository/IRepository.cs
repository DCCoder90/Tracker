namespace Tracker.Data.Repository;

public interface IRepository<T>
{
    T Get();
    void Put(T item);
}
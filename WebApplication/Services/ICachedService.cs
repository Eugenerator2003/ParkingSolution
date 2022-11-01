namespace WebApplication.Services
{
    public interface ICachedService<T> where T : class
    {
        void AddItem(string cacheKey);
        IEnumerable<T> GetItems();
        IEnumerable<T> GetItems(string cacheKey);
    }
}
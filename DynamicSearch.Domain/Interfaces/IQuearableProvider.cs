namespace DynamicSearch.Domain.Interfaces
{
    public interface IQuearableProvider
    {
        IQueryable<object> Get(string schema, string entityName);
    }
}

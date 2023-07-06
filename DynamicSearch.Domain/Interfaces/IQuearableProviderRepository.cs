namespace DynamicSearch.Domain.Interfaces
{
    public interface IQuearableProviderRepository
    {
        IQueryable<object> Get(string clientName, string schemaName, string entityName);
    }
}

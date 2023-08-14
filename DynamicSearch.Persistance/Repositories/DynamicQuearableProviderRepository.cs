using DynamicSearch.Domain.Interfaces;
using DynamicSearch.Persistance.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DynamicSearch.Persistance.Repositories
{
    public class DynamicQuearableProviderRepository : IQuearableProviderRepository
    {
        private readonly IServiceScopeFactory _services;

        public DynamicQuearableProviderRepository(IServiceScopeFactory services)
        {
            _services = services;
        }


        public IQueryable<object> Get(string clientName, string schemaName, string entityName)
        {
            var dynamicContext = _services.CreateScope().ServiceProvider.GetServices(typeof(DbContext)) as IEnumerable<DbContext>;

            return (IQueryable<object>)dynamicContext.ElementAt(0).Query("chat_gpt.Models.Author");
        }
    }
}

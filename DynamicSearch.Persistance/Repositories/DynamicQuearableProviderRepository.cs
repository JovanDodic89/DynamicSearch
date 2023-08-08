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
            var dynamicContext = _services.CreateScope().ServiceProvider.GetService(ConfigureServices.Type) as DbContext;

            var entityTypes = dynamicContext.Model.GetEntityTypes();

            return (IQueryable<object>)dynamicContext.Query("chat_gpt.Models.Author");
        }
    }
}

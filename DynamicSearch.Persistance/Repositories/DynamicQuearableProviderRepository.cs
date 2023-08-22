using DynamicSearch.Domain.Interfaces;
using DynamicSearch.Persistance.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System.Collections.Concurrent;
using System.Reflection;

namespace DynamicSearch.Persistance.Repositories
{
    public class DynamicQuearableProviderRepository : IQuearableProviderRepository
    {
        private readonly IServiceScopeFactory _services;

        public DynamicQuearableProviderRepository(IServiceScopeFactory services)
        {
            _services = services;
        }


        private static ConcurrentDictionary<string, Type> _clientContextTypes = new ConcurrentDictionary<string, Type>();

        public IQueryable<object> Get(string clientName, string schemaName, string entityName)
        {
            DbContext dynamicContext = GetDbContext(clientName);

            if(dynamicContext == null) 
            {
                return null;
            }

            var entityType = dynamicContext.Model
                                           .GetEntityTypes()
                                           .Where(exp => exp.GetSchema().ToLower() == schemaName.ToLower() && exp.ClrType.Name.ToLower() == entityName.ToLower())
                                           .FirstOrDefault();

            if(entityType == null) 
            {
                return null;
            }

            try
            {
                return (IQueryable<object>)dynamicContext.Query(entityType.ClrType.FullName);
            }
            catch
            {
                return null;
            }
        }

        private DbContext GetDbContext(string clientName)
        {
            Type clientType = null;

            if (_clientContextTypes.ContainsKey(clientName))
            {
                clientType = _clientContextTypes[clientName];
            }
            else
            {
                clientType = GetClientTypeFromAssembly(clientName, clientType);
            }

            if (clientType == null)
            {
                return null;
            }

            _clientContextTypes.TryAdd(clientName, clientType);

            return _services.CreateScope().ServiceProvider.GetService(clientType) as DbContext;
        }

        private static Type GetClientTypeFromAssembly(string clientName, Type clientType)
        {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in a.GetTypes())
                {
                    if (t.Name.ToLower() == clientName.ToLower()
                       && t.FullName.ToLower().StartsWith($"{clientName.ToLower()}.context."))
                    {
                        return t;
                    }
                }
            }

            return null;
        }
    }
}

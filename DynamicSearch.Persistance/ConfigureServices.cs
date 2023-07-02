using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DynamicSearch.Persistance
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddPersistanceServices(this IServiceCollection services, IConfiguration configuration)
        {
            AddDbContext(configuration);

            return services;
        }

        private static void AddDbContext(IConfiguration configuration)
        {
            var s = configuration["DatabaseClients"];
        }
    }
}
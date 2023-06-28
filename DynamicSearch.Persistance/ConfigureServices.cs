using Microsoft.Extensions.DependencyInjection;

namespace DynamicSearch.Persistance
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            return services;
        }
    }
}
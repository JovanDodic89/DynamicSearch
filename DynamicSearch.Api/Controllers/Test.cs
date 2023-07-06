using DynamicSearch.Persistance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DynamicSearch.Api.Controllers
{
    [Route("jole/ve1")]
    public class Test : Controller
    {
        public Test(IServiceScopeFactory services)
        {
            var dynamicContext = services.CreateScope().ServiceProvider.GetService(ConfigureServices.Type) as DbContext;

            var entityTypes = dynamicContext.Model.GetEntityTypes();


            Console.WriteLine($"Context contains {entityTypes.Count()} types");

            foreach (var entityType in dynamicContext.Model.GetEntityTypes())
            {
                var items = (IQueryable<object>)dynamicContext.Query(entityType.Name);
               var a = items.Count();
            }
        }

        [Route("test")]
        public IActionResult Index()
        {
            return View();
        }
    }

    public static class DynamicContextExtensions
    {
        public static IQueryable Query(this DbContext context, string entityName) =>
            context.Query(entityName, context.Model.FindEntityType(entityName).ClrType);

        static readonly MethodInfo SetMethod =
            typeof(DbContext).GetMethod(nameof(DbContext.Set), 1, new[] { typeof(string) }) ??
            throw new Exception($"Type not found: DbContext.Set");

        public static IQueryable Query(this DbContext context, string entityName, Type entityType) =>
            (IQueryable)SetMethod.MakeGenericMethod(entityType)?.Invoke(context, new[] { entityName }) ??
            throw new Exception($"Type not found: {entityType.FullName}");
    }

}

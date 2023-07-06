using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.SqlServer.Diagnostics.Internal;
using Microsoft.EntityFrameworkCore.SqlServer.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Runtime.Loader;

namespace DynamicSearch.Persistance
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddPersistanceServices(this IServiceCollection services, IConfiguration configuration)
        {
            AddDbContexts(services, configuration);

            return services;
        }

        private static Type _test;

        public static Type Type { get { return _test; } }

        private static void AddDbContexts(IServiceCollection services, IConfiguration configuration)
        {
            var clients = GetClients(configuration);

            var scaffolder = CreateMssqlScaffolder();

            foreach (var client in clients)
            {
                var connectionString = client.ConnectionString;

                var dbOpts = new DatabaseModelFactoryOptions(client.IncludeTables, client.SchemaNames);
                var modelOpts = new ModelReverseEngineerOptions();
                var codeGenOpts = new ModelCodeGenerationOptions()
                {
                    RootNamespace = client.Name,
                    ContextName = client.Name,
                    ContextNamespace = $"{client.Name}.Context",
                    ModelNamespace = $"{client.Name}.Models",
                    SuppressConnectionStringWarning = true,
                    UseNullableReferenceTypes = true
                };

                var scaffoldedModelSources = scaffolder.ScaffoldModel(connectionString, dbOpts, modelOpts, codeGenOpts);
                var sourceFiles = new List<string> { scaffoldedModelSources.ContextFile.Code };
                sourceFiles.AddRange(scaffoldedModelSources.AdditionalFiles.Select(f => f.Code));

                using var peStream = new MemoryStream();

                var enableLazyLoading = false;
                var result = GenerateCode(sourceFiles, enableLazyLoading).Emit(peStream);

                var assemblyLoadContext = new AssemblyLoadContext("DbContext", isCollectible: !enableLazyLoading);

                peStream.Seek(0, SeekOrigin.Begin);
                var assembly = assemblyLoadContext.LoadFromStream(peStream);

                var type = assembly.GetType($"{client.Name}.Context.{client.Name}");
                _ = type ?? throw new Exception("DataContext type not found");

                var constr = type.GetConstructor(Type.EmptyTypes);
                _ = constr ?? throw new Exception("DataContext ctor not found");

                DbContext dynamicContext = (DbContext)constr.Invoke(null);

                MethodInfo method = typeof(Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions).GetMethods().Where(exp => exp.Name == "AddDbContext").First();
                MethodInfo generic = method.MakeGenericMethod(dynamicContext.GetType());

                _test = dynamicContext.GetType();

                generic.Invoke(services, new[] { services, null, null, null });
            }
        }

        public static MethodInfo GetMethodWithLinq(this Type staticType, string methodName,
    params Type[] paramTypes)
        {
            var methods = from method in staticType.GetMethods()
                          where method.Name == methodName
                                && method.GetParameters()
                                         .Select(parameter => parameter.ParameterType)
                                         .Select(type => type.IsGenericType ?
                                             type.GetGenericTypeDefinition() : type)
                                         .SequenceEqual(paramTypes)
                          select method;
            try
            {
                return methods.SingleOrDefault();
            }
            catch (InvalidOperationException)
            {
                throw new AmbiguousMatchException();
            }
        }

        private static CSharpCompilation GenerateCode(List<string> sourceFiles, bool enableLazyLoading)
        {
            var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp10);

            var parsedSyntaxTrees = sourceFiles.Select(f => SyntaxFactory.ParseSyntaxTree(f, options));

            return CSharpCompilation.Create($"DataContext.dll",
                parsedSyntaxTrees,
                references: CompilationReferences(enableLazyLoading),
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Release,
                    assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
        }

        private static List<MetadataReference> CompilationReferences(bool enableLazyLoading)
        {
            var refs = new List<MetadataReference>();
            var referencedAssemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
            refs.AddRange(referencedAssemblies.Select(a => MetadataReference.CreateFromFile(Assembly.Load(a).Location)));

            refs.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
            refs.Add(MetadataReference.CreateFromFile(typeof(BackingFieldAttribute).Assembly.Location));
            refs.Add(MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0").Location));
            refs.Add(MetadataReference.CreateFromFile(typeof(System.Data.Common.DbConnection).Assembly.Location));
            refs.Add(MetadataReference.CreateFromFile(typeof(System.Linq.Expressions.Expression).Assembly.Location));

            if (enableLazyLoading)
            {
                refs.Add(MetadataReference.CreateFromFile(typeof(ProxiesExtensions).Assembly.Location));
            }

            return refs;
        }


        private static List<Client> GetClients(IConfiguration configuration)
            => configuration.GetSection("DatabaseClients:Clients")
                              .Get<List<Client>>();

        [SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "We need it")]
        static IReverseEngineerScaffolder CreateMssqlScaffolder() =>
            new ServiceCollection()
               .AddEntityFrameworkSqlServer()
               .AddLogging()
               .AddEntityFrameworkDesignTimeServices()
               .AddSingleton<LoggingDefinitions, SqlServerLoggingDefinitions>()
               .AddSingleton<IRelationalTypeMappingSource, SqlServerTypeMappingSource>()
               .AddSingleton<IAnnotationCodeGenerator, AnnotationCodeGenerator>()
               .AddSingleton<IDatabaseModelFactory, SqlServerDatabaseModelFactory>()
               .AddSingleton<IProviderConfigurationCodeGenerator, SqlServerCodeGenerator>()
               .AddSingleton<IScaffoldingModelFactory, RelationalScaffoldingModelFactory>()
               .AddSingleton<IPluralizer, Bricelam.EntityFrameworkCore.Design.Pluralizer>()
               .AddSingleton<ProviderCodeGeneratorDependencies>()
               .AddSingleton<AnnotationCodeGeneratorDependencies>()
               .BuildServiceProvider()
               .GetRequiredService<IReverseEngineerScaffolder>();

        internal class Client
        {
            public string ConnectionString { get; set; }
            public string Name { get; set; }
            public List<string> SchemaNames { get; set; }
            public List<string> IncludeTables { get; set; }
            public List<string> ExcludeTables { get; set; }
        }
    }
}
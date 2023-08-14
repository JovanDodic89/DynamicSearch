using DynamicSearch.Application.Searches.Queries.SearchClient;
using DynamicSearch.Domain.Interfaces;
using DynamicSearch.Persistance.Repositories;
using FluentValidation;
using DynamicSearch.Application;
using DynamicSearch.Api.Filters;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using DynamicSearch.Persistance;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
              .AddJsonFile("configuration/appsettings.json")
              .AddJsonFile($"configuration/appsettings.{builder.Environment.EnvironmentName}.json")
              .AddEnvironmentVariables()
              .Build();

builder.Services.AddMvc(opt => { opt.Filters.Add<ApiExceptionFilterAttribute>(); });
builder.Services.AddPersistanceServices(builder.Configuration);

builder.Services.AddMediatR(c => c.RegisterServicesFromAssemblyContaining<SearchClientQueryCommand>());
builder.Services.AddApplication();

builder.Services.AddScoped<IValidator<SearchClientQueryCommand>, SearchClientQueryCommandValidator>();
builder.Services.AddScoped<IQuearableProviderRepository, DynamicQuearableProviderRepository>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
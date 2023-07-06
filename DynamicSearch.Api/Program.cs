using DynamicSearch.Api.Interceptors;
using DynamicSearch.Application.Searches.Queries.SearchClient;
using DynamicSearch.Domain.Interfaces;
using DynamicSearch.Persistance;
using DynamicSearch.Persistance.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
              .AddJsonFile("configuration/appsettings.json")
              .AddJsonFile($"configuration/appsettings.{builder.Environment.EnvironmentName}.json")
              .AddEnvironmentVariables()
              .Build();


builder.Services.AddMvc();
builder.Services.AddPersistanceServices(builder.Configuration);

builder.Services.AddMediatR(c => c.RegisterServicesFromAssemblyContaining<SearchClientQueryCommand>());
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddScoped<IValidator<SearchClientQueryCommand>, SearchClientQueryCommandValidator>();
builder.Services.AddTransient<IValidatorInterceptor, FluentValidationInterceptor>();

builder.Services.AddScoped<IQuearableProviderRepository, DynamicQuearableProviderRepository>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
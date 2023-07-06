using DynamicSearch.Persistance;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
              .AddJsonFile("configuration/appsettings.json")
              .AddJsonFile($"configuration/appsettings.{builder.Environment.EnvironmentName}.json")
              .AddEnvironmentVariables()
              .Build();

builder.Services.AddPersistanceServices(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
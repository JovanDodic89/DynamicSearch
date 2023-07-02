using DynamicSearch.Persistance;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
              .AddJsonFile("configuration/appsettings.json")
              .AddJsonFile($"configuration/appsettings.{builder.Environment.EnvironmentName}.json")
              .AddEnvironmentVariables()
              .Build();



//var clientsSection = builder.Configuration.GetSection("DatabaseClients:Clients");
//var clients = clientsSection.Get<List<Client>>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
using ConsistentHashingApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Configure logging
using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
    // You can add other logging providers here if needed
    // builder.AddDebug();
    // builder.AddEventLog();
});
// Create a logger
var logger = loggerFactory.CreateLogger<Program>();
var config = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var databaseServers = config.GetSection("DbServers").GetChildren().ToDictionary(_ => _.Key, _ => _.Value);
builder.Services.AddSingleton(new DatabaseSelector(databaseServers.Keys.ToList()));
builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
 {
     var httpContext = serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
     var tenantId = httpContext.Request.Headers["X-TenantId"].FirstOrDefault();
     if (tenantId == null)
     {
         logger.LogError("Tenant Id is null");
         throw new Exception("Tenant Id is null");
     }
     var databaseSelector = serviceProvider.GetRequiredService<DatabaseSelector>();
     var serverName = databaseSelector.GetDatabaseForTenant(tenantId);

     databaseServers.TryGetValue(serverName, out var connectionString);
     logger.LogInformation($"Tenant Id {tenantId} is mapped to {serverName} - {connectionString}");
     options.UseSqlServer(connectionString);
 });

builder.Services.AddHttpContextAccessor();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

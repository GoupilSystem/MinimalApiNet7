using AutoMapper;
using MinimalAPI.Api.Mapping;
using MinimalAPI.Api.Data;
using MinimalAPI.Api.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

var sqlConBuilder = new SqlConnectionStringBuilder() { ConnectionString = builder.Configuration.GetConnectionString("SQLDbConnection") };

// Dependency injections
builder.Services.AddDbContext<ApiContext>(options => options.UseSqlServer(sqlConBuilder.ConnectionString));
builder.Services.AddScoped<IApiRepository, ApiRepository>();
builder.Services.AddScoped(provider => new MapperConfiguration(mc => { mc.AddProfile(new ApiProfiles()); }).CreateMapper());
// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swaggerOptions =>
{
    swaggerOptions.SwaggerDoc("v1", new OpenApiInfo { Title = "MinimalAPI", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// MapGroup will set extensions to all endpoints in the group
var clients = app.MapGroup("")
    .AddEndpointFilterFactory((handlerContext, next) =>
    {
        var loggerFactory = handlerContext.ApplicationServices.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("RequestAuditor");
        return (invocationContext) =>
        {
            logger.LogInformation($"Received a request for: {invocationContext.HttpContext.Request.Path}");
            return next(invocationContext);
        };
    });

// No need for await in NET 7
clients.MapGet("/customers", (IApiRepository repo) => repo.Customers());
clients.MapGet("/customer", (IApiRepository repo, int id) => repo.Customer(id));
clients.MapGet("/purchases", (IApiRepository repo) => repo.Purchases());
clients.MapGet("/customersbycity", (IApiRepository repo, string city) => 
    repo.CustomersByField(new KeyValuePair<FilterKey, string>(FilterKey.City, city)));
clients.MapGet("/customersbypurchasetype", (IApiRepository repo, string purchaseType) =>
    repo.CustomersByField(new KeyValuePair<FilterKey, string>(FilterKey.PurchaseType, purchaseType)));
clients.MapPost("/populatedb", (IApiRepository repo, int amountOfPersons) => repo.PopulateDb(amountOfPersons));

app.Run();

// Required for IntegrationTests otherwise we have a reference problem
public partial class Program { }
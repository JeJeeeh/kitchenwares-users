using KitchenwaresUsers.Models;
using KitchenwaresUsers.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("Db__ConnectionString") ?? 
                       builder.Configuration.GetSection("DevelopmentDatabase")["ConnectionString"];
var collectionName = Environment.GetEnvironmentVariable("Db__CollectionName") ??
                     builder.Configuration.GetSection("DevelopmentDatabase")["CollectionName"];
var databaseName = Environment.GetEnvironmentVariable("Db__DatabaseName") ??
                   builder.Configuration.GetSection("DevelopmentDatabase")["DatabaseName"];

builder.Services.Configure<DatabaseSettings>(options =>
{
    options.ConnectionString = connectionString!;
    options.CollectionName = collectionName!;
    options.DatabaseName = databaseName!;
});

builder.Services.AddSingleton<IUserService, UserService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors(policyBuilder => policyBuilder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);

app.MapControllers();

app.Run();
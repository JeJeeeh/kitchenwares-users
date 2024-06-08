using System.Text;
using KitchenwaresUsers.Models;
using KitchenwaresUsers.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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
builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>();

builder.Services.AddHostedService<RabbitMqHostedService>();

builder.Services.AddControllers();

var jwtIssuer = Environment.GetEnvironmentVariable("Jwt__Issuer") ??
                builder.Configuration.GetSection("DevelopmentJwt")["Issuer"];
var jwtAudience = Environment.GetEnvironmentVariable("Jwt__Audience") ??
                  builder.Configuration.GetSection("DevelopmentJwt")["Audience"];
var jwtSecretKey = Environment.GetEnvironmentVariable("Jwt__SecretKey") ??
                   builder.Configuration.GetSection("DevelopmentJwt")["SecretKey"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey!))
        };
    });


var app = builder.Build();

app.UseCors(policyBuilder => policyBuilder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);

app.MapControllers();

app.Run();
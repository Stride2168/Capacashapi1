using CapacashApi.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using NSwag.AspNetCore;
using Capacash.Application.Services;
using Capacash.Application.Common.Interfaces;
using Capacash.Infrastructure.Repositories;


var builder = WebApplication.CreateBuilder(args);

// Load JWT config
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);
builder.Services.AddInfrastructure(builder.Configuration);

// PLEASE DO NOT Move Authentication & Authorization before `builder.Build()`
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<UserAuthService>(); // Ensure it's registered
builder.Services.AddScoped<IWalletService, WalletService>();  
builder.Services.AddScoped<IWalletRepository, WalletRepository>(); 

// DO NOT Move ALL service registrations before `Build()`
builder.AddKeyVaultIfConfigured();
builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();
builder.Services.AddControllers();
builder.Services.AddScoped<UserAuthService>();
builder.Services.AddInfrastructure();
builder.Services.AddScoped<IKioskRepository, KioskRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITransactionService, TransactionService>();

var app = builder.Build(); 


//  Middleware Execution Order is VERY Important
if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}
else
{
    app.UseHsts();
}


app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSwaggerUi(settings =>
{
    settings.Path = "/api";
    settings.DocumentPath = "/api/specification.json";
});

app.UseExceptionHandler(options => { });

app.UseAuthentication(); //  Authentication middleware BEFORE authorization DOM TF
app.UseAuthorization();

app.Map("/", () => Results.Redirect("/api"));
app.MapEndpoints();
app.MapControllers();
app.UseOpenApi();
app.UseSwaggerUi();


app.Run();

public partial class Program { }

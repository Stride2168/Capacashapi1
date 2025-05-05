using CapacashApi.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using NSwag.AspNetCore;
using Capacash.Application.Common.Interfaces;
using MediatR;
using Capacash.Application.Auth.Commands;
using Capacash.Infrastructure.Services;
using Capacash.Infrastructure.Repositories;
using Capacash.Application.Kiosks.Commands;
using Capacash.Application.Transactions.Queries;

using Capacash.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Load JWT config
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

// Add infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);

// Add authentication
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

// MediatR configuration (only need one registration)
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(RegisterEmployeeCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(KioskLoginCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(GetTransactionsQuery).Assembly);
});

builder.Services.AddScoped<IWalletRepository, WalletRepository>();

// Service registrations
builder.Services.AddScoped<IAuthService, UserAuthService>();
builder.Services.AddScoped<IWalletService, WalletService>();  
builder.Services.AddScoped<IWalletRepository, WalletRepository>(); 
builder.Services.AddScoped<IKioskRepository, KioskRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
// Additional service configurations
builder.AddKeyVaultIfConfigured();
builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();

builder.Services.AddControllers();
builder.Services.AddAuthorization();

var app = builder.Build();

// Middleware pipeline
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

app.UseAuthentication();
app.UseAuthorization();

app.Map("/", () => Results.Redirect("/api"));
app.MapEndpoints();
app.MapControllers();
app.UseOpenApi();
app.UseSwaggerUi();

app.Run();

public partial class Program { }
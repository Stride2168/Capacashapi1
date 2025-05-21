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
using CapacashApi.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Capacash.Domain.Entities;
using Hangfire;
using CapacashApi.Infrastructure.Hangfire;
using Capacash.Application.Wallets.Commands.MonthlyRegenerateCredit;

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
            IssuerSigningKey = new SymmetricSecurityKey(key),
             RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
        };
         options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse(); 
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync("{\"error\": \"Unauthorized\"}");
            } };
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
builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.User.RequireUniqueEmail = true;
        // Add more user options if needed
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


// Additional service configurations
builder.AddKeyVaultIfConfigured();
builder.Services.AddScoped<IRecurringRegenerator, RecurringRegenerator>();

builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();
builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddHangfireServer();

builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddScoped<RecurringMediatorExecutor>();
// Register CORS with named policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
        policy.WithOrigins("http://localhost:4200", "http://localhost:8100")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

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
app.Use(async (context, next) =>
{
    Console.WriteLine($"Requested: {context.Request.Method} {context.Request.Path}");
    await next();
});
app.UseCors("FrontendPolicy");

app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() }
});

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
RecurringJob.AddOrUpdate<RecurringMediatorExecutor>(
    "monthly-credit-regeneration",
    executor => executor.ExecuteMonthlyRegeneration(),
    Cron.Daily
);


app.Run();

public partial class Program { }
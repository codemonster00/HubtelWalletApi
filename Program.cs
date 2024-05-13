using HubTelWalletApi.Context;
using HubTelWalletApi.Interfaces;
using HubTelWalletApi.Models;
using HubTelWalletApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using System;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using HubTelWalletApi.Middleware;
using Microsoft.OpenApi.Models;





namespace HubTelWalletApi
{
    public class Program
    {
        private static ConfigurationManager _configuration;
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Debug()
           .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // Ignore Microsoft logs below Warning level
           .Enrich.FromLogContext()
           .WriteTo.Console()
           .WriteTo.File("log/log.txt", rollingInterval: RollingInterval.Day) // Log to a file
           .CreateBootstrapLogger();
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // var configuration = builder.Configuration;
            _configuration = builder.Configuration;
           

            _configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            builder.Host.UseSerilog();
            builder.Services.AddAutoMapper(typeof(Program));


            builder.Services.AddControllers();
            builder.Services.AddDbContext<AppDbContext>(options =>
         options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlServerOptionsAction: sqloptions =>
         {


             sqloptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
         }));
            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "HubTelWallet API", Version = "v1" });

            });
            builder.Services.AddScoped<IOtpSender, UssdOtpSender>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IWalletRepository, WalletRepository>();

            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings.GetValue<string>("SecretKey");
            var issuer = jwtSettings.GetValue<string>("Issuer");
            var audience = jwtSettings.GetValue<string>("Audience");

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(options =>
           {
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateIssuerSigningKey = true,
                   ValidIssuer = issuer,
                   ValidAudience = audience,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
               };
           });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseAuthentication();
            app.UseAuthorization();

           
            app.MapControllers();

            app.Run();
        }

        public static JwtSettings GetJwtSettings()
        {
            return _configuration.GetSection("JwtSettings").Get<JwtSettings>();
        }
    }
}

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using System.Reflection;

namespace OnlineShop.Shared.Common.Bootstraps
{
    public static class AppBootstrap
    {
        public static void AddFeaturesApp(WebApplicationBuilder builder)
        {
            var appBootstrapBuilder = new AppBootstrapBuilder();

            var config = appBootstrapBuilder.GetConfiguration();
            appBootstrapBuilder.CreateLogger(config.Item1, config.Item2);
            builder.Host.UseSerilog();
            builder.Services.AddSingleton(config.Item2);
            builder.Services.AddSerilog();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, x =>
            {
                x.Authority = "https://localhost:5001";
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false
                };
            });
        }
    }
    internal class AppBootstrapBuilder
    {
        public void CreateLogger(string environment, IConfigurationRoot configuration)
        {
            Log.Logger = new LoggerConfiguration()
                            .Enrich.FromLogContext()
                            .Enrich.WithMachineName()
                            .Enrich.WithExceptionDetails()
                            .WriteTo.Debug()
                            .WriteTo.Elasticsearch(ConfigureElasticSink(configuration, environment))
                            .Enrich.WithProperty("Environment", environment)
                            .CreateLogger();
        }
        private ElasticsearchSinkOptions ConfigureElasticSink(IConfigurationRoot configuration, string environment)
        {
            return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
            };
        }
        public (string, IConfigurationRoot) GetConfiguration()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(
                    $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                    optional: true)
                .Build();
            return (environment, configuration);
        }

    }
}

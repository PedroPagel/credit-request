using Cofidis.Credit.Api.Middlewares;
using Cofidis.Credit.Domain.Services.Credits.Requests;
using Cofidis.Credit.Domain.Services.DigitalMobileKey;
using Cofidis.Credit.Domain.Services.Risks.Analysis;
using Cofidis.Credit.Domain.Services.Users;
using Cofidis.Credit.Infrastructure.Configurations;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Cofidis.Credit.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Configure(builder);

            var app = builder.Build();

            ConfigureMiddleware(app);

            app.Run();
        }

        public static void Configure(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<IDigitalMobileKeyService, DigitalMobileKeyService>();
            builder.Services.AddScoped<ICreditRequestService, CreditRequestService>();
            builder.Services.AddScoped<IRiskAnalysisService, RiskAnalysisService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddLogging();

            builder.Services.ConfigureDataLayer(builder.Configuration, builder.Environment);
            builder.Services.ResolveDependencies(builder.Configuration);
            builder.Services.AddAutoMapper(typeof(Program).Assembly);

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Codifis Credit Request API",
                    Description = "API Documentation for Codifis Credit Request Application",
                });

                // Add comments from XML documentation
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            });

            builder.Services.AddSingleton<ExceptionHandlerMiddleware>();
        }

        public static void ConfigureMiddleware(WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}
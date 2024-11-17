using Cofidis.Credit.Domain.Options;
using Cofidis.Credit.Domain.Repositories;
using Cofidis.Credit.Domain.Services.Notificator;
using Cofidis.Credit.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cofidis.Credit.Infrastructure.Configurations
{
    public static class InjectionConfiguration
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<INotificator, Notificator>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRiskAnalysisRepository, RiskAnalysisRepository>();
            services.AddScoped<ICreditRequestRepository, CreditRequestRepository>();

            services.Configure<RiskAnalysisOptions>(opt =>
            {
                opt.MediumRiskThreshold = configuration.GetValue("RiskAnalysis:MediumRiskThreshold", 20);
                opt.HighRiskThreshold = configuration.GetValue("RiskAnalysis:HighRiskThreshold", 50);
            });

            return services;
        }
    }
}

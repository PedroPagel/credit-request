using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;

namespace Cofidis.Credit.Infrastructure.Configurations
{
    public static class ConfiguraAppSettings
    {
        public static IApplicationBuilder ConfigureApp(this IApplicationBuilder app)
        {
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseMiddleware<ExceptionHandlerMiddleware>();

            return app;
        }
    }
}

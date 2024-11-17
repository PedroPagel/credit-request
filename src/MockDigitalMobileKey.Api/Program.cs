namespace MockDigitalMobileKey.Api
{
    public class Program
    {
        public static void Main()
        {
            var builder = CreateWebApplicationBuilder();
            var app = builder.Build();

            Configure(app);

            app.Run();
        }

        public static WebApplicationBuilder CreateWebApplicationBuilder()
        {
            var builder = WebApplication.CreateBuilder();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            return builder;
        }

        public static void Configure(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
        }
    }
}
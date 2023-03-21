namespace SpannerWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.            
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            //builder.Services.AddHostedService<DatabaseService>();
            
            ConsoleLogger.Install();
            if (!string.IsNullOrWhiteSpace(builder.Configuration["ApplicationCredentialsPath"]))
            {
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", builder.Configuration["ApplicationCredentialsPath"]);
            }

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            // Ensure that the appsettings.json is updated and uncomment the following three line and comment out app.Run()
            // before deploying in CloudRun.

            //var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
            //var url = $"http://0.0.0.0:{port}";
            //app.Run(url);

            app.Run();
        }
    }
}
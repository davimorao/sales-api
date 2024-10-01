using Serilog;
using System.Net;

namespace Sales.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.Debug()
                .CreateLogger();

            try
            {
                Log.Information("Starting up the application");

                var builder = WebApplication.CreateBuilder(args);

                builder.Host.UseSerilog();

                // Configurar Kestrel para escutar nas portas HTTP e HTTPS
                builder.WebHost.ConfigureKestrel(options =>
                {
                    options.ListenAnyIP(8080); // Porta HTTP

                    // Configuração da porta HTTPS
                    var certPassword = Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Password") ?? "pocsales";
                    var certPath = Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Path") ?? "/app/certificates/aspnetapp.pfx";

                    options.ListenAnyIP(8081, listenOptions =>
                    {
                        listenOptions.UseHttps(certPath, certPassword);
                    });
                });

                // Adicionar serviços ao contêiner.
                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                builder.Services.AddUseCases();
                builder.Services.AddMessageQueue();
                builder.Services.AddPersistence();

                var app = builder.Build();

                // Configurar o pipeline de requisição HTTP.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseAuthorization();

                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}

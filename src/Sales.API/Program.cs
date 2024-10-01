using Serilog;

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
                //builder.WebHost.ConfigureKestrel(options =>
                //{
                //    options.ListenAnyIP(8080); // Porta HTTP

                //    var certPassword = Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Password") ?? "pocsales";
                //    var certPath = Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Path") ?? "/app/certificates/aspnetapp.pfx";

                //    options.ListenAnyIP(8081, listenOptions =>
                //    {
                //        listenOptions.UseHttps(certPath, certPassword);
                //    });
                //});

                // Adicionar serviços ao contêiner.
                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                builder.Services.AddUseCases();
                builder.Services.AddMessageQueue();
                builder.Services.AddPersistence();

                var app = builder.Build();

                // Redirecionar HTTP para HTTPS
                //if (!app.Environment.IsDevelopment())
                //{
                //    app.UseHttpsRedirection();
                //}
                //else
                //{
                //    app.Use(async (context, next) =>
                //    {
                //        if (!context.Request.IsHttps || context.Request.Host.Port == 8080)
                //        {
                //            await next();
                //        }
                //        else
                //        {
                //            var httpsUrl = $"http://{context.Request.Host.Host}:8080{context.Request.Path}{context.Request.QueryString}";
                //            context.Response.Redirect(httpsUrl, permanent: true);
                //        }
                //    });
                //}



                // Swagger e SwaggerUI em ambiente de desenvolvimento
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                    //app.MapGet("/", context =>
                    //{
                    //    context.Response.Redirect("/swagger/index.html");
                    //    return Task.CompletedTask;
                    //});
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

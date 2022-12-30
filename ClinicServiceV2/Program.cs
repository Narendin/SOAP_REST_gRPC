using ClinicService.Data.EF;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Net;

namespace ClinicServiceV2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Listen(IPAddress.Any, 5100, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
                options.Listen(IPAddress.Any, 5101, listenOptions => listenOptions.Protocols = HttpProtocols.Http1);
            });

            var services = builder.Services;

            services.AddDbContext<ApplicationContext>(options => ApplicationContextHelper.ConfigureDbContextOptions(options, builder.Configuration));

            services.AddGrpc().AddJsonTranscoding();
            services.AddGrpcSwagger();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "ClinicServiceV2",
                    Version = "v1"
                });

                var filePath = Path.Combine(AppContext.BaseDirectory, "ClinicServiceV2.xml");
                c.IncludeXmlComments(filePath);
                c.IncludeGrpcXmlComments(filePath, includeControllerXmlComments: true);
            });

            // https://learn.microsoft.com/ru-ru/aspnet/core/grpc/json-transcoding-openapi?view=aspnetcore-7.0


            var app = builder.Build();

            UpdateDatabase(app);

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"));
            }

            app.UseRouting();
            app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
            app.MapGrpcService<Services.Implementation.ClinicService>().EnableGrpcWeb();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client");

            app.Run();
        }

        private static void UpdateDatabase(IApplicationBuilder app)
        {
            try
            {
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    using (var context = serviceScope.ServiceProvider.GetService<ApplicationContext>())
                    {
                        context?.Database?.Migrate();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Diagnostics;

namespace EmotionApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            services.AddApiVersioning();
            services.AddSingleton<IConfiguration>(Configuration);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            /*
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {

                    var error = context.Features.Get<IExceptionHandlerFeature>();
                    if (error != null)
                    {
                        var ex = error.Error;
                        var statusCode = 500;
                        if (ex is NotFoundException)
                        {
                            statusCode = 404;
                        }

                        context.Response.StatusCode = statusCode;
                        context.Response.ContentType = "application/json";

                        await context.Response.WriteAsync(new ErroServico()
                        {
                            Code = statusCode,
                            Message = ex.Message
                        }.ToString(), Encoding.UTF8);
                    }
                });
            });
            */

            app.UseCors(builder =>
            {
                builder.WithOrigins("*")
                       .WithMethods("*")
                       .AllowAnyHeader();
            });

            app.UseMvc();
        }
    }
}

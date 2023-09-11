using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SanService.Infrastructure;
using SanService.Infrastructure.Filters;

namespace SanService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // глобально - все сервисы MVC - и контроллеры, и Razor Page
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(SimpleResourceFilter)); // подключение по типу                

                // альтернативный вариант подключения
                //options.Filters.Add(new SimpleResourceFilter()); // подключение по объекту
            });
            
            services.AddRazorPages().AddMvcOptions(options =>
            {
                options.Filters.Add(new SimpleResourceFilter());    // подключение по объекту
                //options.Filters.Add(typeof(SimpleResourceFilter)); // подключение по типу
            });

            services.AddControllers();
            var key = Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // валидация ключа безопасности
                    ValidateIssuerSigningKey = true,
                    // установка ключа безопасности
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    // будет ли валидироваться время существования
                    ValidateLifetime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


            //app.UseForwardedHeaders(new ForwardedHeadersOptions
            //{
            //    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            //});
        }
    }
}

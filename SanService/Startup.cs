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
            // ��������� - ��� ������� MVC - � �����������, � Razor Page
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(SimpleResourceFilter)); // ����������� �� ����                

                // �������������� ������� �����������
                //options.Filters.Add(new SimpleResourceFilter()); // ����������� �� �������
            });
            
            services.AddRazorPages().AddMvcOptions(options =>
            {
                options.Filters.Add(new SimpleResourceFilter());    // ����������� �� �������
                //options.Filters.Add(typeof(SimpleResourceFilter)); // ����������� �� ����
            });

            services.AddControllers();
            var key = Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // ��������� ����� ������������ 
                    ValidateIssuerSigningKey = true,
                    // ��������� ����� ������������
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    // ����� �� �������������� ����� �������������
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

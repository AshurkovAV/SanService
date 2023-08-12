using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;

namespace SanService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //При инициализации хоста у объекта WebHostBuilder вызывается метод UseKestrel(),
                    //который позволяет задействовать Kestrel. Несмотря на то, что по умолчанию исходный
                    //код в файле Program.cs не содержит этого вызова, этот метод вызывается автоматически.
                    webBuilder.UseStartup<Startup>()
                    .UseKestrel(options =>
                    {
                        //При развертывании на Windows Kestrel может применять IIS в качестве прокси - сервера,
                        //а при развертывании на Linux как прокси - серверы могут использоваться
                        //Apache и Nginx.Но также Kestrel может работать самостоятельно внтури своего процесса без IIS.
                        //options.Limits.MaxConcurrentConnections = 100;
                        //options.Limits.MaxRequestBodySize = 10 * 1024;
                        //options.Limits.MinRequestBodyDataRate =
                        //    new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                        //options.Limits.MinResponseDataRate =
                        //    new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                        options.Listen(IPAddress.Parse("192.168.10.7"), 5001);
                    });
                });
    }
}

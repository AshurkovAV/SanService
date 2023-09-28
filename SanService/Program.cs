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
                    //��� ������������� ����� � ������� WebHostBuilder ���������� ����� UseKestrel(),
                    //������� ��������� ������������� Kestrel. �������� �� ��, ��� �� ��������� ��������
                    //��� � ����� Program.cs �� �������� ����� ������, ���� ����� ���������� �������������.
                    webBuilder.UseStartup<Startup>()
                    .UseKestrel(options =>
                    {
                        //��� ������������� �� Windows Kestrel ����� ��������� IIS � �������� ������ - �������,
                        //� ��� ������������� �� Linux ��� ������ - ������� ����� ��������������
                        //Apache � Nginx.�� ����� Kestrel ����� �������� �������������� ������ ������ �������� ��� IIS.
                        //options.Limits.MaxConcurrentConnections = 100;
                        //options.Limits.MaxRequestBodySize = 10 * 1024;
                        //options.Limits.MinRequestBodyDataRate =
                        //    new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                        //options.Limits.MinResponseDataRate =
                        //    new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                        options.Listen(IPAddress.Parse("192.168.88.149"), 5001);
                    });
                });
    }
}

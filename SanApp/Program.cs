using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SanatoriumEntities.Models.Services;
using SanService.Infrastructure.Filters;
using System.Web.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    
    //options.Conventions.AddPageRoute("/Authorization/Index", "");
});


builder.Services.ConfigureApplicationCookie(configure: config => config.LoginPath = "/Login");

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new SimpleResourceFilter());    // подключение по объекту    
});


var app = builder.Build();



app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/robots.txt"))
    {
        var robotsTxtPath = Path.Combine(app.Environment.ContentRootPath, $"robots.{app.Environment.EnvironmentName}.txt");
        string output = "User-agent: *  \nDisallow: /administrator/" +
        "\r\nDisallow: /bin/" +
        "\r\nDisallow: /cache/" +
        "\r\nDisallow: /cli/" +
        "\r\nDisallow: /components/" +
        "\r\nDisallow: /includes/" +
        "\r\nDisallow: /installation/" +
        "\r\nDisallow: /language/" +
        "\r\nDisallow: /layouts/" +
        "\r\nDisallow: /libraries/" +
        "\r\nDisallow: /logs/" +
        "\r\nDisallow: /modules/" +
        "\r\nDisallow: /plugins/" +
        "\r\nDisallow: /tmp/ ";
        if (File.Exists(robotsTxtPath))
        {
            output = await File.ReadAllTextAsync(robotsTxtPath);
        }
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync(output);
    }
    else await next();
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();



app.MapRazorPages();

app.Run();
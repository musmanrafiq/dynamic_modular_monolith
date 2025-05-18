using Em.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EM.WebHostApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add MVC support for host app
            builder.Services.AddControllersWithViews();

            // Load plugins (DLLs)
            var pluginManager = new PluginManager();
            pluginManager.LoadPlugins(AppContext.BaseDirectory + "\\Modules", builder.Services);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();

            // Map plugin routes
            pluginManager.MapEndpoints(app);

            // Map fallback for main app routes
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

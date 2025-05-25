using Em.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace EM.Modules.Feature.Outflow
{
    public class OutflowModule : IFeatureModule
    {
        public string Name => "Outflows";

        public void RegisterServices(IServiceCollection services)
        {
            /*services.AddDbContext<UsersDbContext>(options =>
                options.UseSqlServer("YourConnectionString")); // can be injected via config later
            */
            services.AddControllersWithViews()
                .AddApplicationPart(typeof(OutflowModule).Assembly)
                .AddRazorRuntimeCompilation(); // Enables view discovery in DLLs
        }

        public void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllerRoute(
                name: "UserModule",
                pattern: "users/{action=Index}/{id?}",
                defaults: new { controller = "User" });
        }
    }
}

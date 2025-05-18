using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Em.Common
{
    public interface IFeatureModule
    {
        string Name { get; }
        void RegisterServices(IServiceCollection services);
        void MapEndpoints(IEndpointRouteBuilder endpoints);
    }
}

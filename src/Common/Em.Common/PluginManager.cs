using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Em.Common
{
    public class PluginManager
    {
        private readonly List<IFeatureModule> _modules = new();
        private readonly List<PluginLoadContext> _loadContexts = new();

        /// <summary>
        /// Loads plugin assemblies from the given folder and registers services from feature modules.
        /// </summary>
        public void LoadPlugins(string pluginFolder, IServiceCollection services)
        {
            if (!Directory.Exists(pluginFolder))
            {
                Console.WriteLine($"❌ Plugin folder '{pluginFolder}' does not exist.");
                return;
            }

            var pluginFiles = Directory.GetFiles(pluginFolder, "*.dll");

            foreach (var pluginPath in pluginFiles)
            {
                // Skip views assembly – handled automatically by ASP.NET Core MVC
                if (pluginPath.Contains(".Views")) continue;

                try
                {
                    if (pluginPath.Contains(".Feature"))
                    {
                        var loadContext = new PluginLoadContext(pluginPath);
                        _loadContexts.Add(loadContext); // Save context if unloading is needed later

                        var assembly = loadContext.LoadFromAssemblyPath(pluginPath);

                        var moduleTypes = assembly.GetTypes()
                         //.Where(t => typeof(IFeatureModule).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass);
                         .Where(t => t.Name.EndsWith("Module"));

                        foreach (var type in moduleTypes)
                        {
                            Console.WriteLine($"{type.FullName} - From Assembly: {type.Assembly.FullName}");
                            Console.WriteLine($"Implements IFeatureModule: {typeof(IFeatureModule).IsAssignableFrom(type)}");
                        }

                        foreach (var type in moduleTypes)
                        {
                            var aa = Activator.CreateInstance(type);

                            if (Activator.CreateInstance(type) is IFeatureModule module)
                            {
                                module.RegisterServices(services);
                                _modules.Add(module);

                                Console.WriteLine($"✅ Loaded module: {module.Name}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Failed to load plugin '{pluginPath}': {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Maps all plugin-defined endpoints to the application.
        /// </summary>
        public void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            foreach (var module in _modules)
            {
                try
                {
                    module.MapEndpoints(endpoints);
                    Console.WriteLine($"🔗 Endpoints mapped for module: {module.Name}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Failed to map endpoints for module '{module.Name}': {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Optionally dispose and unload all plugin contexts.
        /// </summary>
        public void UnloadPlugins()
        {
            foreach (var context in _loadContexts)
            {
                context.Unload(); // Only works if PluginLoadContext is collectible
            }

            _loadContexts.Clear();
            _modules.Clear();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}

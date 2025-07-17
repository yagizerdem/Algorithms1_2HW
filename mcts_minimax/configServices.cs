using Microsoft.Extensions.DependencyInjection;

static class ConfigServices
{
    public static void Configure()
    {
        var services = new ServiceCollection();
        // Register services here
        services.AddSingleton<AppContext>();
        services.AddSingleton<Drawer>();
        // build the service provider
        var serviceProvider = services.BuildServiceProvider();
        // Set the service provider in a static locator for global access
        ServiceLocator.Instance = serviceProvider;
    }
}
static class ServiceLocator
{
    public static IServiceProvider Instance { get; set; }
}

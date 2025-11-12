
namespace UpdateManager.DotNetUpgradeLibrary.Extensions;

public static class ConfigurationExtensions
{
    public static string GetLibraryPath(this IConfiguration configuration)
    {
        string value = configuration[NetUpgradeConfigurationKeys.LibraryPath] ?? throw new ConfigurationKeyNotFoundException("The library path not found");
        return value;
    }
    public static string GetNetPath(this IConfiguration configuration)
    {
        string value = configuration[NetUpgradeConfigurationKeys.NetPath] ?? throw new ConfigurationKeyNotFoundException("The net library path not found");
        return value;
    }
    // New extension method to get the upgrade process config path
    public static string GetUpgradeProcessConfigPath(this IConfiguration configuration)
    {
        string value = configuration[NetUpgradeConfigurationKeys.UpgradeProcessConfigKey]
            ?? throw new ConfigurationKeyNotFoundException("The upgrade configuration value not found");
        return value;
    }
}
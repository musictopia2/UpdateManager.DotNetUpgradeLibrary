namespace UpdateManager.DotNetUpgradeLibrary.Extensions;
public static class ConfigurationExtensions
{
    extension(IConfiguration configuration)
    {
        public string LibraryPath
        {
            get
            {
                string value = configuration[NetUpgradeConfigurationKeys.LibraryPath] ?? throw new ConfigurationKeyNotFoundException("The library path not found");
                return value;
            }
        }
        public string NetPath
        {
            get
            {
                string value = configuration[NetUpgradeConfigurationKeys.NetPath] ?? throw new ConfigurationKeyNotFoundException("The net library path not found");
                return value;
            }
        }
        public string UpgradeProcessConfigPath
        {
            get
            {
                string value = configuration[NetUpgradeConfigurationKeys.UpgradeProcessConfigKey]
                    ?? throw new ConfigurationKeyNotFoundException("The upgrade configuration value not found");
                return value;
            }
        }
    }
}
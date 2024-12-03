namespace UpdateManager.DotNetUpgradeLibrary.Utilities;
public static class NetUpgradeConfigurationKeys
{
    public static string LibraryPath => "LibraryPath";
    public static string NetPath => "NetPath";
    public static string UpgradeProcessConfigKey => "UpgradeConfigKey"; // New key for upgrade config (paths + custom settings)
}
namespace UpdateManager.DotNetUpgradeLibrary.Extensions;
internal static class DotNetUpgradeExtensions
{
    public static bool NeedsToUpdateVersion(this DotNetUpgradeConfigurationModel config)
    {
        int targetVersion = config.NetVersion + 1;
        return DotNetVersionChecker.IsDotNetVersionInstalled(targetVersion);
    }
}
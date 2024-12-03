namespace UpdateManager.DotNetUpgradeLibrary.Extensions;
internal static class DotNetUpgradeExtensions
{
    public static bool NeedsToUpdateVersion(this DotNetUpgradeConfigurationModel config)
    {
        int possibleNewVersion = config.NetVersion + 1;
        return DotNetVersionChecker.IsDotNetVersionInstalled(possibleNewVersion);
    }
}
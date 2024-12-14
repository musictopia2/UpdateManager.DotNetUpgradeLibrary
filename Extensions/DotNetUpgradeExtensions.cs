using UpdateManager.DotNetUpgradeLibrary.Utilities;

namespace UpdateManager.DotNetUpgradeLibrary.Extensions;
internal static class DotNetUpgradeExtensions
{
    public static bool NeedsToUpdateVersion(this int version)
    {
        int targetVersion = version + 1;
        return DotNetVersionChecker.IsDotNetVersionInstalled(targetVersion);
    }
}
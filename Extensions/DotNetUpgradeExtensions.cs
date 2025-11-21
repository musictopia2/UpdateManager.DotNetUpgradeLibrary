namespace UpdateManager.DotNetUpgradeLibrary.Extensions;
internal static class DotNetUpgradeExtensions
{
    extension(int version)
    {
        public bool NeedsToUpdateVersion()
        {
            int targetVersion = version + 1;
            return DotNetVersionChecker.IsDotNetVersionInstalled(targetVersion);
        }
    }
}
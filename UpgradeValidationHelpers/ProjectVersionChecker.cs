namespace UpdateManager.DotNetUpgradeLibrary.UpgradeValidationHelpers;
public static class ProjectVersionChecker
{
    public static bool IsUpgraded(string csProj)
    {
        CsProjEditor editor = new(csProj);
        string versionUsed = editor.VersionUsed();
        string latestVersion = bb1.Configuration!.GetNetVersion();
        return latestVersion == versionUsed;
    }
}
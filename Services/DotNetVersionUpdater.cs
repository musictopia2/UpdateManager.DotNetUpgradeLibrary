namespace UpdateManager.DotNetUpgradeLibrary.Services;
public class DotNetVersionUpdater : IDotNetVersionUpdater
{
    private static readonly string _netPath = bb1.Configuration!.GetNetPath();
    public static void ReloadConfiguration()
    {
        if (bb1.Configuration is IConfigurationRoot root)
        {
            root.Reload();
        }
    }
    async Task IDotNetVersionUpdater.UpdateNetVersionAsync(int newNetVersion)
    {
        string oldFileContent = await ff1.AllTextAsync(_netPath);
        string newContent = GetContents(newNetVersion);
        await ff1.WriteAllTextAsync(_netPath, newContent);
        ReloadConfiguration(); //trying this.  hopefully fixes the problem for next year.
        bool success = VerifyFileContent(newNetVersion);
        if (!success)
        {
            // If the verification fails, revert to the old file content
            await ff1.WriteAllTextAsync(_netPath, oldFileContent);
            // Raise an exception after rolling back
            throw new InvalidOperationException(
                "Version info was not correctly updated. The changes were rolled back. " +
                "If using the 'AddTextFile' extension to load the configuration, ensure that the 'ReloadOnChange' parameter is set to 'true'. " +
                "This ensures that changes to the configuration file are reflected in the application dynamically. " +
                "Example: builder.AddTextFile('yourpath', false, true);");
        }
    }
    private static bool VerifyFileContent(int versionExpected)
    {
        // Retrieve the version from IConfiguration after it has been reloaded
        string versionFoundString = bb1.Configuration!.GetNetVersion();

        // Attempt to parse the NetVersion from IConfiguration
        bool rets = int.TryParse(versionFoundString, out int versionFoundInt);
        if (!rets)
        {
            return false; // If the NetVersion is not an integer, it's invalid
        }

        // Verify that the NetVersion matches the expected value
        return versionFoundInt == versionExpected;
    }

    private static string GetContents(int newVersion)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"NetVersion\t{newVersion}");

        // Return the constructed text content
        return sb.ToString();
    }
}
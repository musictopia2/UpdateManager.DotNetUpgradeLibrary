namespace UpdateManager.DotNetUpgradeLibrary.UpgradeValidationHelpers;

// POCO class to represent the runtimeconfig.json structure
internal class RuntimeConfig
{
    public RuntimeOptions? RuntimeOptions { get; set; }

    // Method to check if the major version matches the expected one
    public bool IsVersionCorrect(string expectedVersion)
    {
        // Extract the major version from the expected version
        if (int.TryParse(expectedVersion, out int expectedMajorVersion))
        {
            if (RuntimeOptions!.Tfm.StartsWith("net") == false)
            {
                return false;
            }
            HtmlParser parses = new();
            parses.Body = RuntimeOptions.Tfm;
            string item = parses.GetSomeInfo("net", ".");
            if (int.TryParse(item, out int versionFound))
            {
                return versionFound == expectedMajorVersion;
            }
        }
        // If no match, return false
        return false;
    }
}

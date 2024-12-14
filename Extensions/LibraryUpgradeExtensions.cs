namespace UpdateManager.DotNetUpgradeLibrary.Extensions;
public static class LibraryUpgradeExtensions
{
    // Method to decrement a version string (e.g., 1.0.86 -> 1.0.85)
    //may be needed even for cases where someone is creating their own yearly helpers but need this.
    private static string DecrementVersion(this string version)
    {
        var versionParts = version.Split('.');

        if (versionParts.Length != 3)
        {
            throw new ArgumentException("Invalid version format");
        }
        if (!int.TryParse(versionParts[2], out int patchVersion))
        {
            throw new ArgumentException("Invalid patch version format");
        }
        patchVersion--; // Decrement the patch version

        // If patch version is negative, handle accordingly (e.g., reset to 0)
        if (patchVersion <= 0)
        {
            throw new CustomBasicException($"Invalid version decrement: The patch version cannot go below 1.0.1. Current version: {version}. This indicates a misconfiguration or an unexpected versioning error.");
        }
        // Rebuild the version string
        return $"{versionParts[0]}.{versionParts[1]}.{patchVersion}";
    }
    public static async Task<bool> AlreadyUpgradedAsync(this LibraryNetUpgradeModel upgradeModel, DotNetUpgradeBasicConfig dotNetModel)
    {
        if (dotNetModel.IsTestMode)
        {
            return false; //since its testing, go ahead and go through the process of upgrading no matter what since its only testing anyways
        }
        string directory = Path.GetDirectoryName(upgradeModel.CsProjPath)!;
        Console.WriteLine(directory);
        if (DotNetVersionHelper.IsExpectedVersionInReleaseBuild(directory) == false)
        {
            return false;
        }

        string netVersion = bb1.Configuration!.GetNetVersion();
        if (upgradeModel.PackageType == EnumFeedType.Public)
        {
            //has to check something like 9.0.1.  if there on public nuget, then already done period.
            string upgradeVersion = $"{netVersion}.0.1";
            string packageId = upgradeModel.GetPackageID();
            bool isPublicPackageUpgraded = await CheckPublicFeedAsync(packageId, upgradeVersion);
            if (isPublicPackageUpgraded)
            {
                return true; // If public feed is upgraded, return true
            }
            bool isStagingPackageUpgraded = await CheckStagingFeedAsync(upgradeModel);
            if (isStagingPackageUpgraded)
            {
                return true;
            }
            if (upgradeModel.Development == false)
            {
                return false;
            }
            return await CheckDevelopmentFeedAsync(upgradeModel, netVersion);
        }
        if (upgradeModel.Version == "1.0.1")
        {
            return false; //this means since this would have been incremented, then means you need this upgraded.
        }
        // Check the local production feed
        bool isLocalUpgraded = CheckLocalProductionFeed(upgradeModel, netVersion);
        if (isLocalUpgraded)
        {
            return true;
        }
        if (upgradeModel.Development == false)
        {
            return false;
        }
        // Check the development feed if it's in development
        return await CheckDevelopmentFeedAsync(upgradeModel, netVersion);
    }
    private static async Task<bool> CheckPublicFeedAsync(string packageName, string upgradeVersion)
    {
        // Check if the public feed contains the upgraded version
        return await NuGetPackageChecker.IsPublicPackageAvailableAsync(packageName, upgradeVersion);
    }
    // Method to get the .NET version (e.g., "9" from "net9.0") from the .nuspec file
    private static string GetNetVersionFromNuspec(string packageName, string packagePath)
    {
        string nuspecPath = Path.Combine(packagePath, $"{packageName.ToLower()}.nuspec");

        if (File.Exists(nuspecPath))
        {
            XDocument nuspecXml = XDocument.Load(nuspecPath);

            // Find the targetFramework attribute under the dependencies group
            var targetFrameworkElement = nuspecXml.Descendants()
                                                   .Where(e => e.Name.LocalName == "group" && e.Attribute("targetFramework") != null)
                                                   .FirstOrDefault();
            if (targetFrameworkElement != null)
            {
                // Extract the version from "net9.0" -> "9"
                var targetFramework = targetFrameworkElement.Attribute("targetFramework")?.Value;
                if (targetFramework?.StartsWith("net") == true)
                {
                    // Remove the "net" prefix, and then split by "." to get the major version
                    string version = targetFramework.Substring(3); // "9.0"
                    string[] versionParts = version.Split('.'); // ["9", "0"]
                    return versionParts[0]; // Return the first part, "9"
                }
                throw new CustomBasicException($"The .nuspec file at {nuspecPath} does not contain a valid target framework version. Please ensure the file is structured correctly.");
            }
            else
            {
                throw new CustomBasicException($"The .nuspec file at {nuspecPath} does not contain a valid target framework version. Please ensure the file is structured correctly.");
            }
        }
        else
        {
            throw new CustomBasicException($"The path {nuspecPath} does not exist");
        }
    }
    private static bool CheckLocalProductionFeed(LibraryNetUpgradeModel upgradeModel, string netVersion)
    {
        string feedPath = bb1.Configuration!.GetPrivatePackagePath();
        string lastVersion = upgradeModel.Version.DecrementVersion();
        return FinishGettingNetVersion(feedPath, upgradeModel.PackageName, lastVersion, netVersion);
    }
    private static async Task<bool> CheckStagingFeedAsync(LibraryNetUpgradeModel upgradeModel)
    {
        string feedPath = bb1.Configuration!.GetStagingPackagePath();
        string packageName = upgradeModel.GetPackageID();
        return await LocalNuGetFeedManager.PackageExistsAsync(feedPath, packageName);
    }
    private static async Task<bool> CheckDevelopmentFeedAsync(LibraryNetUpgradeModel upgradeModel, string netVersion)
    {
        string feedPath = bb1.Configuration!.GetDevelopmentPackagePath();
        if (upgradeModel.PackageType == EnumFeedType.Local)
        {
            string lastVersion;
            lastVersion = upgradeModel.Version.DecrementVersion();
            return FinishGettingNetVersion(feedPath, upgradeModel.PackageName, lastVersion, netVersion);
        }
        string packageName = upgradeModel.GetPackageID();
        return await LocalNuGetFeedManager.PackageExistsAsync(feedPath, packageName);
    }
    private static bool FinishGettingNetVersion(string feedPath, string packageName, string lastVersion, string netVersion)
    {
        string packagePath = Path.Combine(feedPath, packageName, lastVersion);
        string packageNetVersion = GetNetVersionFromNuspec(packageName, packagePath);
        return packageNetVersion == netVersion;
    }
}
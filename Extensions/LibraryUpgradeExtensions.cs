﻿namespace UpdateManager.DotNetUpgradeLibrary.Extensions;
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
    public static async Task<bool> AlreadyUpgradedAsync(this LibraryNetUpgradeModel upgradeModel, DotNetUpgradeConfigurationModel dotNetModel)
    {
        if (dotNetModel.IsTestMode)
        {
            return false; //since its testing, go ahead and go through the process of upgrading no matter what since its only testing anyways
        }
        string netVersion = bb1.Configuration!.GetNetVersion();
        if (upgradeModel.PackageType == EnumFeedType.Public)
        {
            //has to check something like 9.0.1.  if there on public nuget, then already done period.
            string upgradeVersion = $"{netVersion}.0.1";
            bool isPublicPackageUpgraded = await CheckPublicFeedAsync(upgradeModel.PackageName, upgradeVersion);

            if (isPublicPackageUpgraded)
            {
                return true; // If public feed is upgraded, return true
            }
            bool isStagingPackageUpgraded = CheckStagingFeed(upgradeModel, netVersion);
            if (isStagingPackageUpgraded)
            {
                return true;
            }
            if (upgradeModel.Development == false)
            {
                return false;
            }
            return CheckDevelopmentFeed(upgradeModel, netVersion);
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
        return CheckDevelopmentFeed(upgradeModel, netVersion);
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
                    return targetFramework.Substring(3); // returns "9" from "net9.0"
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
    private static bool CheckStagingFeed(LibraryNetUpgradeModel upgradeModel, string netVersion)
    {
        string feedPath = bb1.Configuration!.GetPrivatePackagePath();
        string lastVersion = upgradeModel.Version.DecrementVersion();
        return FinishGettingNetVersion(feedPath, upgradeModel.PackageName, lastVersion, netVersion);
    }
    private static bool CheckDevelopmentFeed(LibraryNetUpgradeModel upgradeModel, string netVersion)
    {
        string feedPath = bb1.Configuration!.GetDevelopmentPackagePath();
        string lastVersion;
        if (upgradeModel.PackageType == EnumFeedType.Local)
        {
            lastVersion = upgradeModel.Version.DecrementVersion();
        }
        else
        {
            lastVersion = upgradeModel.Version;
        }
        return FinishGettingNetVersion(feedPath, upgradeModel.PackageName, lastVersion, netVersion);
    }
    private static bool FinishGettingNetVersion(string feedPath, string packageName, string lastVersion, string netVersion)
    {
        string packagePath = Path.Combine(feedPath, packageName, lastVersion);
        string packageNetVersion = GetNetVersionFromNuspec(packageName, packagePath);
        return packageNetVersion == netVersion;
    }
}
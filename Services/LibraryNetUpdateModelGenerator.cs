namespace UpdateManager.DotNetUpgradeLibrary.Services;
public class LibraryNetUpdateModelGenerator(IPackagesContext packageContext) : ILibraryNetUpdateModelGenerator
{
    async Task<BasicList<LibraryNetUpgradeModel>> ILibraryNetUpdateModelGenerator.CreateLibraryNetUpdateModelListAsync()
    {
        BasicList<NuGetPackageModel> packages = await packageContext.GetPackagesAsync();
        packages.RemoveAllAndObtain(x => x.TemporarilyIgnore || x.Framework == EnumTargetFramework.NetStandard);
        BasicList<LibraryNetUpgradeModel> output = [];
        // Check if we need to filter packages for testing
        bool includeOnlyTests = LibraryInclusionGlobals.LibrariesToIncludeForTest.Count > 0;
        foreach (var package in packages)
        {
            if (package.TemporarilyIgnore)
            {
                continue; //ignore those.
            }
            if (includeOnlyTests && !LibraryInclusionGlobals.LibrariesToIncludeForTest
                .Any(testLibrary => string.Equals(testLibrary, package.PackageName, StringComparison.OrdinalIgnoreCase)))
            {
                continue; // Skip this package as it's not in the test list
            }
            LibraryNetUpgradeModel upgrade = new()
            {
                PackageType = package.FeedType,
                PackageName = package.PackageName,
                CsProjPath = package.CsProjPath,
                NugetPackagePath = package.NugetPackagePath,
                Status = EnumDotNetUpgradeStatus.None,
                Development = package.Development,
                PrefixForPackageName = package.PrefixForPackageName
            };
            upgrade.Version = package.FeedType == EnumFeedType.Local
                ? package.Version.IncrementMinorVersion()
                : "0.0.1"; // Placeholder version for non-local packages, should be updated later
            output.Add(upgrade);
        }
        output = LibraryDependencyResolver.ResolveDependencies(output); //needs to resolve dependencies as part of the steps.
        return output;
    }
}
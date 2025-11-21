namespace UpdateManager.DotNetUpgradeLibrary.Services;
public class PackageFeedManager(INugetPacker packer) : IPackageFeedManager
{
    async Task<bool> IPackageFeedManager.PublishPackageToFeedAsync(LibraryNetUpgradeModel upgradeModel, string feedPath, CancellationToken cancellationToken)
    {
        bool rets = await packer.CreateNugetPackageAsync(upgradeModel, true, cancellationToken);
        if (rets == false)
        {
            return false;
        }
        var files = ff1.FileList(upgradeModel.NugetPackagePath);
        // Remove all files that do not end with ".nupkg" (case-insensitive)
        files.RemoveAllOnly(x => !x.EndsWith(".nupkg", StringComparison.OrdinalIgnoreCase));
        // Check if there's exactly one .nupkg file in the list
        if (files.Count != 1)
        {
            Console.WriteLine($"Error: Expected 1 .nupkg file, but found {files.Count}.");
            return false;
        }
        string nugetFile = files.Single();
        nugetFile = ff1.FullFile(nugetFile);
        rets = await LocalNuGetFeedUploader.UploadPrivateNugetPackageAsync(feedPath, upgradeModel.NugetPackagePath, nugetFile, cancellationToken);
        return rets;
    }
}
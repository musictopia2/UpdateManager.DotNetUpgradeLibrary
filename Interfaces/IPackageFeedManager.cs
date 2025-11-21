namespace UpdateManager.DotNetUpgradeLibrary.Interfaces;
public interface IPackageFeedManager
{
    Task<bool> PublishPackageToFeedAsync(LibraryNetUpgradeModel upgradeModel, string feedPath, CancellationToken cancellationToken = default);  // Publish to a specialized feed
}
namespace UpdateManager.DotNetUpgradeLibrary.Interfaces;
public interface IPackageFeedManager
{
    void InitializeFeed(DotNetUpgradeBasicConfig upgradeModel); // Initialize feeds for yearly updates, skip if already present
    Task<bool> PublishPackageToFeedAsync(LibraryNetUpgradeModel upgradeModel, string feedPath, CancellationToken cancellationToken = default);  // Publish to a specialized feed
    void CleanupFeed(string feedPath); // Cleanup temporary feeds for the yearly update
    void ClearFeed(string feedPath); // Clear feed before testing (for the yearly process)
}
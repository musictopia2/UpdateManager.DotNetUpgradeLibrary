namespace UpdateManager.DotNetUpgradeLibrary.Interfaces;
public interface ILibraryDotNetUpgradeCommitter
{
    Task<bool> CommitAndPushToGitHubAsync(LibraryNetUpgradeModel updateModel, DotNetUpgradeConfigurationModel versionUpgradeModel, CancellationToken cancellationToken = default);
}
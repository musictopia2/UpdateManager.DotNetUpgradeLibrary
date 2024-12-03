namespace UpdateManager.DotNetUpgradeLibrary.Interfaces;
public interface ILibraryDotNetUpgradeCommitter
{
    Task<bool> CommitAndPushToGitHubAsync(LibraryNetUpgradeModel updateModel, DotNetUpgradeBasicConfig versionUpgradeModel, CancellationToken cancellationToken = default);
}
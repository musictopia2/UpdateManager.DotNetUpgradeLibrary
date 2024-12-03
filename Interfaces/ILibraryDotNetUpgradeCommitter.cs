namespace UpdateManager.DotNetUpgradeLibrary.Interfaces;
public interface ILibraryDotNetUpgradeCommitter
{
    Task<bool> CommitAndPushToGitHubAsync(LibraryNetUpdateModel updateModel, DotNetVersionUpgradeModel versionUpgradeModel, CancellationToken cancellationToken = default);
}
namespace UpdateManager.DotNetUpgradeLibrary.Interfaces;
public interface ILibraryDotNetUpgradeCommitter
{
    Task<bool> CommitAndPushToGitHubAsync(LibraryNetUpgradeModel updateModel, CancellationToken cancellationToken = default);
}
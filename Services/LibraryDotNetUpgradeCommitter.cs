namespace UpdateManager.DotNetUpgradeLibrary.Services;
public class LibraryDotNetUpgradeCommitter(IUpgradePhaseHandler handler) : ILibraryDotNetUpgradeCommitter
{
    async Task<bool> ILibraryDotNetUpgradeCommitter.CommitAndPushToGitHubAsync(LibraryNetUpgradeModel updateModel, CancellationToken cancellationToken)
    {
        bool rets;
        rets = await handler.HandleCommitAsync(updateModel);
        if (rets)
        {
            return true; //because someone else is handling this later.
        }
        rets = await GitHubCommitter.CommitAndPushToGitHubAsync(updateModel.CsProjPath, $"Updated To {bb1.Configuration!.GetNetVersion()}", cancellationToken);
        return rets;
    }
}
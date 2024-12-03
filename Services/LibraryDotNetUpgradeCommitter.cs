﻿namespace UpdateManager.DotNetUpgradeLibrary.Services;
public class LibraryDotNetUpgradeCommitter(IPostUpgradeProcessHandler handler) : ILibraryDotNetUpgradeCommitter
{
    async Task<bool> ILibraryDotNetUpgradeCommitter.CommitAndPushToGitHubAsync(LibraryNetUpgradeModel updateModel, DotNetUpgradeBasicConfig versionUpgradeModel, CancellationToken cancellationToken)
    {
        if (versionUpgradeModel.IsTestMode)
        {
            throw new CustomBasicException("You cannot commit to github because its testing.  Run out of test mode in order to commit to github");
        }
        bool rets;
        rets = await handler.HandleCommitAsync(updateModel, versionUpgradeModel);
        if (rets)
        {
            return true; //because someone else is handling this later.
        }
        rets = await GitHubCommitter.CommitAndPushToGitHubAsync(updateModel.CsProjPath, $"Updated To {versionUpgradeModel.NetVersion}", cancellationToken);
        return rets;
    }
}
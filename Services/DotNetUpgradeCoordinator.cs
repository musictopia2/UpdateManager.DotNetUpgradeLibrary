namespace UpdateManager.DotNetUpgradeLibrary.Services;
public class DotNetUpgradeCoordinator(
    IFeedPathResolver feedPathResolver,
    ITestFileManager testFileManager,
    IDotNetVersionInfoRepository dotNetVersionInfoManager,
    IPreUpgradeProcessHandler preUpgradeProcessHandler,
    INetVersionUpdateContext netVersionUpdateContext,
    ILibraryDotNetUpgraderBuild libraryDotNetUpgraderBuild,
    IPackageFeedManager yearlyFeedManager,
    ILibraryDotNetUpgradeCommitter libraryDotNetUpgradeCommitter,
    IPostBuildCommandStrategy postBuildCommandStrategy,
    IPostUpgradeProcessHandler postUpgradeProcessHandler
    )
{
    public async Task<UpgradeProcessState> GetUpgradeStatusAsync(BasicList<LibraryNetUpgradeModel> libraries)
    {
        DotNetUpgradeConfigurationModel netConfig = await dotNetVersionInfoManager.GetVersionInfoAsync();
        bool needsUpdate;
        needsUpdate = netConfig.NeedsToUpdateVersion();
        if (needsUpdate)
        {
            return new(EnumUpgradePhase.PendingUpdate, netConfig);
        }
        if (libraries.All(x => x.Status == EnumDotNetUpgradeStatus.Completed))
        {
            return new(EnumUpgradePhase.UpgradeCompleted, netConfig);
        }
        if (libraries.All(x => x.Status == EnumDotNetUpgradeStatus.None))
        {
            return new(EnumUpgradePhase.NotStarted, netConfig);
        }
        return new(EnumUpgradePhase.InProgress, netConfig);
    }
    public async Task<BasicList<LibraryNetUpgradeModel>> GetLibrariesAsync(bool testing)
    {
        if (netVersionUpdateContext.IsLibraryDataPresent() == false)
        {
            return await netVersionUpdateContext.ReprocessLibrariesForUpdateAsync();
        }
        return await netVersionUpdateContext.GetLibrariesForUpdateAsync();
    }
    public async Task UpdateNetVersionAsync(DotNetUpgradeConfigurationModel netConfig)
    {
        // Get the highest installed .NET version (throws exception if not found)
        int latestVersion = DotNetVersionChecker.GetHighestInstalledDotNetVersion()
            ?? throw new CustomBasicException("Unable to update to the latest version because no .NET version was found.");
        // Update the configuration with the latest version
        netConfig.NetVersion = latestVersion;
        if (netConfig.IsTestMode == false)
        {
            // Reset flags for new version (pre and post upgrade)
            await preUpgradeProcessHandler.ResetFlagsForNewVersionAsync(netConfig);
            await postUpgradeProcessHandler.ResetFlagsForNewVersionAsync(netConfig); //to let any post processes know you have a new version.
        }
        // Save updated version info
        await dotNetVersionInfoManager.SaveVersionInfoAsync(netConfig);
        // Reset library context (likely clears or prepares libraries for the new version)
        await netVersionUpdateContext.ResetLibraryAsync();
    }
}
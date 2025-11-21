namespace UpdateManager.DotNetUpgradeLibrary.Services;
public class DotNetUpgradeCoordinator(
    IFeedPathResolver feedPathResolver,
    IDotNetVersionUpdater versionUpdater,
    INetVersionUpdateContext netVersionUpdateContext,
    IPackageFeedManager packageFeedManager,
    IPackagesContext packageContext, //i assume i need this so it can update this.
    IBranchValidationService branchService,
    ILibraryDotNetUpgraderBuild libraryDotNetUpgraderBuild,
    ILibraryDotNetUpgradeCommitter libraryDotNetUpgradeCommitter,
    IUpgradePhaseHandler upgradeProcessHandler
    )
{
    public async Task<UpgradeProcessState> GetUpgradeStatusAsync(BasicList<LibraryNetUpgradeModel> libraries)
    {
        // Get the configuration for the .NET upgrade
        // Initialize any possible custom pre-upgrade processes
        await InitializePossibleCustomProcessesAsync();
        // Check if the system needs an update
        bool needsUpdate;
        int version = int.Parse(bb1.Configuration!.GetNetVersion());
        needsUpdate = version.NeedsToUpdateVersion();
        if (needsUpdate)
        {
            // If an update is required, return the "PendingUpdate" phase
            return new(EnumUpgradePhase.PendingUpdate, false);
        }
        bool rets;
        rets = libraries.All(x => x.Status == EnumDotNetUpgradeStatus.Completed);

        // If it's not in test mode, check if pre-upgrade processes are required
        if (upgradeProcessHandler.Are1PreUpgradeProcessesNeeded())
        {
            // Pre-upgrade processes are required and will run in non-test mode
            return new(EnumUpgradePhase.PreUpgradePending, rets);
        }
        // If all libraries have completed the upgrade, return "UpgradeCompleted"
        if (rets)
        {
            return new(EnumUpgradePhase.UpgradeCompleted, true);
        }
        // If all libraries are still in "None" status (not yet started), it means pre-upgrade processes are done
        if (libraries.All(x => x.Status == EnumDotNetUpgradeStatus.None))
        {
            return new(EnumUpgradePhase.PreUpgradeCompleted, false);
        }
        // If any library has a status other than "Completed", it means that the upgrade process is still in progress
        // for at least some libraries. Some libraries may be fully upgraded (status "Completed"), while others are still in progress.
        return new(EnumUpgradePhase.UpgradeInProgress, false);
    }
    public async Task SaveLibrariesAsync(BasicList<LibraryNetUpgradeModel> libraries)
    {
        await netVersionUpdateContext.SaveUpdatedLibrariesAsync(libraries);
    }
    private async Task InitializePossibleCustomProcessesAsync()
    {
        await upgradeProcessHandler.InitAsync();
    }
    public async Task<BasicList<LibraryNetUpgradeModel>> GetLibrariesAsync()
    {
        if (netVersionUpdateContext.IsLibraryDataPresent() == false)
        {
            return await netVersionUpdateContext.ReprocessLibrariesForUpdateAsync();
        }
        return await netVersionUpdateContext.GetLibrariesForUpdateAsync();
    }
    public async Task UpdateNetVersionAsync()
    {
        // Get the highest installed .NET version (throws exception if not found)
        int latestVersion = DotNetVersionChecker.GetHighestInstalledDotNetVersion()
            ?? throw new CustomBasicException("Unable to update to the latest version because no .NET version was found.");
        // Update the configuration with the latest version
        // Reset flags for new version (pre and post upgrade)
        await upgradeProcessHandler.ResetFlagsForNewVersionAsync();
        // Save updated version info
        await versionUpdater.UpdateNetVersionAsync(latestVersion);
        // Reset library context (likely clears or prepares libraries for the new version)
        await netVersionUpdateContext.ResetLibraryAsync();
    }
    public async Task<bool> RunPreUpgradeProcessesAsync()
    {
        bool rets = await upgradeProcessHandler.Run1PreUpgradeProcessesAsync();
        return rets;
    }
    public async Task ProcessLibraryUpdateAsync(LibraryNetUpgradeModel library, BasicList<LibraryNetUpgradeModel> libraries)
    {
        if (library.Status == EnumDotNetUpgradeStatus.Completed)
        {
            return;
        }
        if (library.PackageType == EnumFeedType.Public)
        {
            library.Version = $"{bb1.Configuration!.GetNetVersion()}.0.1";
        }
        if (library.Status == EnumDotNetUpgradeStatus.None)
        {
            if (await branchService.ValidateBranchAsync(library))
            {
                library.Status = EnumDotNetUpgradeStatus.BranchCheckedOut;
            }
            else
            {
                Console.WriteLine($"Failed to check out branch for {library.PackageName}");
                return; //stop here i think.
            }
        }
        if (library.Status == EnumDotNetUpgradeStatus.BranchCheckedOut)
        {
            //this means to build the library.
            //only can do if it has not been done previously.

            
            if (library.PackageType == EnumFeedType.Public)
            {
                string version = library.Version.StartMajorVersion();
                await packageContext.UpdatePackageVersionAsync(library.PackageName, version);
            }
            if (await libraryDotNetUpgraderBuild.BuildLibraryAsync(library, libraries) == false)
            {
                Console.WriteLine($"Build failed for {library.PackageName}");
                return;
            }
            library.Status = EnumDotNetUpgradeStatus.Built;
        }
        
        bool successful;
        //need advice about this part (?)
        
        if (library.Status == EnumDotNetUpgradeStatus.Built)
        {
            string feedPath = feedPathResolver.GetFeedPath(library);
            if (feedPath == "")
            {
                library.Status = EnumDotNetUpgradeStatus.WaitingForGitHub;
            }
            else
            {
                successful = await packageFeedManager.PublishPackageToFeedAsync(library, feedPath);
                if (successful == false)
                {
                    Console.WriteLine($"Published failed for {library.PackageName}");
                    return;
                }
                library.Status = EnumDotNetUpgradeStatus.WaitingForGitHub; //for now, waiting for github.
            }
        }
        if (library.Status == EnumDotNetUpgradeStatus.WaitingForGitHub)
        {
            successful = await libraryDotNetUpgradeCommitter.CommitAndPushToGitHubAsync(library);
            if (successful == false)
            {
                Console.WriteLine($"Failed to commit to GitHub for {library.PackageName}");
                return;
            }
            library.Status = EnumDotNetUpgradeStatus.Completed;
        }
    }
    public async Task FinalizePostUpgradeProcessesAsync()
    {
        if (upgradeProcessHandler.ArePostUpgradeProcessesNeeded() == false)
        {
            Console.WriteLine("No post-upgrade processes needed. The upgrade is now fully complete.");
            return;
        }
        await upgradeProcessHandler.RunPostUpgradeProcessesAsync();
        if (upgradeProcessHandler.ArePostUpgradeProcessesNeeded() == false)
        {
            Console.WriteLine("Post-upgrade processes completed successfully. The upgrade is now fully finished.");
            return;
        }
        Console.WriteLine("Post-upgrade processes did not complete successfully. Please review the logs or troubleshooting steps.");
    }
}
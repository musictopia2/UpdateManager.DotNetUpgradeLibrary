namespace UpdateManager.DotNetUpgradeLibrary.Services;
public class DotNetUpgradeCoordinator(
    IFeedPathResolver feedPathResolver,
    ITestFileManager testFileManager,
    IDotNetUpgradeConfigReader dotNetUpgradeConfig,
    IDotNetVersionUpdater versionUpdater,
    INetVersionUpdateContext netVersionUpdateContext,
    IPackagesContext packageContext, //i assume i need this so it can update this.
    IBranchValidationService branchService,
    ILibraryDotNetUpgraderBuild libraryDotNetUpgraderBuild,
    IPackageFeedManager packageFeedManager,
    ILibraryDotNetUpgradeCommitter libraryDotNetUpgradeCommitter,
    IUpgradePhaseHandler upgradeProcessHandler
    )
{
    public async Task<UpgradeProcessState> GetUpgradeStatusAsync(BasicList<LibraryNetUpgradeModel> libraries)
    {
        // Get the configuration for the .NET upgrade
        DotNetUpgradeBasicConfig netConfig = await dotNetUpgradeConfig.GetConfigAsync();
        // Initialize any possible custom pre-upgrade processes
        await InitializePossibleCustomProcessesAsync(netConfig);
        // Check if the system needs an update
        bool needsUpdate;
        int version = int.Parse(bb1.Configuration!.GetNetVersion());
        needsUpdate = version.NeedsToUpdateVersion();
        if (needsUpdate)
        {
            // If an update is required, return the "PendingUpdate" phase
            return new(EnumUpgradePhase.PendingUpdate, netConfig, false);
        }
        bool rets;
        rets = libraries.All(x => x.Status == EnumDotNetUpgradeStatus.Completed);

        // If it's not in test mode, check if pre-upgrade processes are required
        if (netConfig.IsTestMode == false)
        {
            if (upgradeProcessHandler.Are1PreUpgradeProcessesNeeded())
            {
                // Pre-upgrade processes are required and will run in non-test mode
                return new(EnumUpgradePhase.PreUpgradePending, netConfig, rets);
            }
        }
        // If all libraries have completed the upgrade, return "UpgradeCompleted"
        if (rets)
        {
            return new(EnumUpgradePhase.UpgradeCompleted, netConfig, true);
        }
        // If all libraries are still in "None" status (not yet started), it means pre-upgrade processes are done
        if (libraries.All(x => x.Status == EnumDotNetUpgradeStatus.None))
        {
            return new(EnumUpgradePhase.PreUpgradeCompleted, netConfig, false);
        }
        // If any library has a status other than "Completed", it means that the upgrade process is still in progress
        // for at least some libraries. Some libraries may be fully upgraded (status "Completed"), while others are still in progress.
        return new(EnumUpgradePhase.UpgradeInProgress, netConfig, false);
    }
    public async Task SaveLibrariesAsync(BasicList<LibraryNetUpgradeModel> libraries)
    {
        await netVersionUpdateContext.SaveUpdatedLibrariesAsync(libraries);
    }
    public void InitalizeFeeds(DotNetUpgradeBasicConfig config)
    {
        packageFeedManager.InitializeFeed(config); //i think the first step is to initialize feeds no matter what.
    }
    private async Task InitializePossibleCustomProcessesAsync(DotNetUpgradeBasicConfig config)
    {
        if (config.IsTestMode)
        {
            return; // because can't do in test mode.
        }
        await upgradeProcessHandler.InitAsync();
    }
    public void ClearFeedsOnce(DotNetUpgradeBasicConfig netConfig)
    {
        if (netConfig.IsTestMode)
        {
            packageFeedManager.ClearFeed(netConfig.TestLocalFeedPath);
            packageFeedManager.ClearFeed(netConfig.TestPublicFeedPath);
        }
    }
    public async Task<BasicList<LibraryNetUpgradeModel>> GetLibrariesAsync()
    {
        if (netVersionUpdateContext.IsLibraryDataPresent() == false)
        {
            return await netVersionUpdateContext.ReprocessLibrariesForUpdateAsync();
        }
        return await netVersionUpdateContext.GetLibrariesForUpdateAsync();
    }
    public async Task UpdateNetVersionAsync(DotNetUpgradeBasicConfig netConfig)
    {
        // Get the highest installed .NET version (throws exception if not found)
        int latestVersion = DotNetVersionChecker.GetHighestInstalledDotNetVersion()
            ?? throw new CustomBasicException("Unable to update to the latest version because no .NET version was found.");
        // Update the configuration with the latest version
        
        if (netConfig.IsTestMode == false)
        {
            // Reset flags for new version (pre and post upgrade)
            await upgradeProcessHandler.ResetFlagsForNewVersionAsync();
        }
        // Save updated version info
        await versionUpdater.UpdateNetVersionAsync(latestVersion);
        // Reset library context (likely clears or prepares libraries for the new version)
        await netVersionUpdateContext.ResetLibraryAsync();
    }
    private static string GetBackupFilePath(LibraryNetUpgradeModel libraryConfig)
    {
        // Get the directory, file name without extension, and extension separately
        string directory = Path.GetDirectoryName(libraryConfig.CsProjPath)!; // Get directory part of the path
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(libraryConfig.CsProjPath); // File name without extension
        string extension = Path.GetExtension(libraryConfig.CsProjPath); // File extension (e.g., .csproj)

        // Construct the backup file path: same directory, file name with ".backup" suffix, and original extension
        return Path.Combine(directory, $"{fileNameWithoutExtension}.backup{extension}");
    }
    private void BackupIfNeeded(LibraryNetUpgradeModel libraryConfig, bool isTestMode)
    {
        if (libraryConfig.Status != EnumDotNetUpgradeStatus.None)
        {
            return;
        }
        string backupFilePath = GetBackupFilePath(libraryConfig);
        if (isTestMode)
        {
            // For test mode: Always try to restore from backup if it exists, otherwise create a new backup
            if (testFileManager.FileExists(backupFilePath))
            {
                //Console.WriteLine("[TEST] Restoring from backup.");
                testFileManager.CopyFile(backupFilePath, libraryConfig.CsProjPath); // Restore from backup
            }
            else
            {
                //Console.WriteLine("[TEST] No backup found. Creating backup.");
                testFileManager.CopyFile(libraryConfig.CsProjPath, backupFilePath); // Create a backup for the first time
            }
        }
        else
        {
            // In production, always restore from backup if it exists (never create a new backup)
            if (testFileManager.FileExists(backupFilePath))
            {
                //Console.WriteLine("[PROD] Restoring from backup.");
                testFileManager.CopyFile(backupFilePath, libraryConfig.CsProjPath); // Restore from backup
            }
        }
    }
    private void DeleteBackup(LibraryNetUpgradeModel libraryConfig)
    {
        // Delete backup after production publish
        string backupFilePath = GetBackupFilePath(libraryConfig);

        if (testFileManager.FileExists(backupFilePath))
        {
            testFileManager.DeleteFile(backupFilePath);
            //Console.WriteLine("[PROD] Backup deleted after successful publish.");
        }
    }
    public static BasicList<LibraryNetUpgradeModel> GetBuildOrder(BasicList<LibraryNetUpgradeModel> libraries, string rootName)
    {
        var sortedLibraries = new BasicList<LibraryNetUpgradeModel>();
        var processedLibraries = new HashSet<string>(); // Keeps track of libraries that have been processed

        // Start the recursion with the root library
        AddLibraryToBuildOrder(rootName, libraries, sortedLibraries, processedLibraries);

        // Now loop through the rest of the libraries that weren't visited as dependencies
        foreach (var library in libraries)
        {
            if (!processedLibraries.Contains(library.PackageName))
            {
                AddLibraryToBuildOrder(library.PackageName, libraries, sortedLibraries, processedLibraries);
            }
        }

        // Ensure that all libraries are processed and none are missed
        if (sortedLibraries.Count != libraries.Count)
        {
            throw new InvalidOperationException($"The number of libraries in the sorted order ({sortedLibraries.Count}) does not match the number of libraries in the input list ({libraries.Count}).");
        }

        return sortedLibraries;
    }
    private static void AddLibraryToBuildOrder(string rootName, BasicList<LibraryNetUpgradeModel> libraries, BasicList<LibraryNetUpgradeModel> sortedLibraries, HashSet<string> processedLibraries)
    {
        // Skip if already processed
        if (processedLibraries.Contains(rootName))
        {
            return;
        }

        // Find the library by name
        var library = libraries.FirstOrDefault(l => l.PackageName == rootName);
        if (library == null)
        {
            return; // If no library is found, do nothing (you could throw an exception here if it's unexpected)
        }

        // Mark the library as processed
        processedLibraries.Add(rootName);

        // Add dependencies first (recursive call)
        foreach (var dependency in library.Dependencies)
        {
            AddLibraryToBuildOrder(dependency, libraries, sortedLibraries, processedLibraries);
        }

        // After processing dependencies, add the current library to the sorted list
        sortedLibraries.Add(library);
    }
    public async Task<bool> RunPreUpgradeProcessesAsync(DotNetUpgradeBasicConfig config)
    {
        if (config.IsTestMode)
        {
            throw new CustomBasicException(
                "Pre-upgrade processes cannot be run in test mode. " +
                "For example, upgrading the post command to the latest .NET version without modifying other dependencies is not supported in test mode. " +
                "Please ensure that 'GetUpgradeStatusAsync' returned the correct status and that this method was not called in error.");
        }
        bool rets = await upgradeProcessHandler.Run1PreUpgradeProcessesAsync();
        return rets;
    }
    public async Task ProcessLibraryUpdateAsync(LibraryNetUpgradeModel library, DotNetUpgradeBasicConfig config, BasicList<LibraryNetUpgradeModel> libraries)
    {
        if (library.Status == EnumDotNetUpgradeStatus.Completed)
        {
            return;
        }
        if (library.PackageType == EnumFeedType.Public)
        {
            library.Version = $"{bb1.Configuration!.GetNetVersion()}.0.1";
        }
        BackupIfNeeded(library, config.IsTestMode);
        if (config.IsTestMode == false)
        {
            //you should have already tested it before doing for reals anyways.
            DeleteBackup(library); //looks like if production, then after restoring, forced to risk it.  because otherwise, private packages won't build.
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

            if (await libraryDotNetUpgraderBuild.AlreadyUpgradedAsync(library, config))
            {
                library.Status = EnumDotNetUpgradeStatus.Completed; //this means if everything is upgraded, then its already complete period.
                return; //i think nothing else now.
            }
            if (library.PackageType == EnumFeedType.Public)
            {
                string version = library.Version.StartMajorVersion();
                await packageContext.UpdatePackageVersionAsync(library.PackageName, version);
            }
            if (await libraryDotNetUpgraderBuild.BuildLibraryAsync(library, config, libraries) == false)
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
            string feedPath = feedPathResolver.GetFeedPath(library, config);
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
                if (config.IsTestMode == false)
                {
                    library.Status = EnumDotNetUpgradeStatus.WaitingForGitHub; //for now, waiting for github.
                }
                else
                {
                    library.Status = EnumDotNetUpgradeStatus.Completed;
                    return;
                }
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
    public async Task FinalizePostUpgradeProcessesAsync(DotNetUpgradeBasicConfig config)
    {
        if (config.IsTestMode)
        {
            Console.WriteLine("Upgrade completed successfully in Test Mode. Post-upgrade processes were skipped.");
            return;
        }
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
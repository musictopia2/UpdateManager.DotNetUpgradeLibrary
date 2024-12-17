namespace UpdateManager.DotNetUpgradeLibrary.Services;
public class UpgradePhaseManager : IUpgradePhaseHandler
{
    private BasicList<IUpgradePhaseHandler> _managers;

    // Constructor now correctly uses the factory property to create the list of handlers
    public UpgradePhaseManager(IUpgradePhaseFactory factory)
    {
        _managers = factory.CreateUpgradePhases;  // Use the property (not method)
    }

    bool IUpgradePhaseHandler.ArePostUpgradeProcessesNeeded()
    {
        // Return true if any manager indicates post-upgrade processes are needed
        return _managers.Any(manager => manager.ArePostUpgradeProcessesNeeded());
    }

    bool IUpgradePhaseHandler.Are1PreUpgradeProcessesNeeded()
    {
        // Return true if any manager indicates pre-upgrade processes are needed
        return _managers.Any(manager => manager.Are1PreUpgradeProcessesNeeded());
    }

    // Commit handler: Returns true as soon as any manager handles the commit
    async Task<bool> IUpgradePhaseHandler.HandleCommitAsync(LibraryNetUpgradeModel netUpdateModel)
    {
        foreach (var manager in _managers)
        {
            // If any manager handles the commit, stop further processing
            if (await manager.HandleCommitAsync(netUpdateModel))
            {
                return true;
            }
        }
        return false; // None of the managers handled the commit
    }

    // Initialize all managers
    async Task IUpgradePhaseHandler.InitAsync()
    {
        foreach (var manager in _managers)
        {
            await manager.InitAsync();
        }
    }

    // Reset flags for new version across all managers
    async Task IUpgradePhaseHandler.ResetFlagsForNewVersionAsync()
    {
        foreach (var manager in _managers)
        {
            await manager.ResetFlagsForNewVersionAsync();
        }
    }

    // Run post-upgrade processes across all managers
    async Task<bool> IUpgradePhaseHandler.RunPostUpgradeProcessesAsync()
    {
        foreach (var manager in _managers)
        {
            if (manager.Are1PreUpgradeProcessesNeeded())
            {
                if (await manager.RunPostUpgradeProcessesAsync() == false)
                {
                    Console.WriteLine($"Post upgrade failed for {manager.GetType().Name}");
                    return false;
                }
            }
        }
        return true;
    }

    // Run pre-upgrade processes across all managers
    async Task<bool> IUpgradePhaseHandler.Run1PreUpgradeProcessesAsync()
    {
        foreach (var manager in _managers)
        {
            if (manager.Are1PreUpgradeProcessesNeeded())
            {
                if (await manager.Run1PreUpgradeProcessesAsync() == false)
                {
                    Console.WriteLine($"Pre upgrade failed for {manager.GetType().Name}");
                    return false;
                }
            }
        }
        return true;
    }
}
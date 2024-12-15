namespace UpdateManager.DotNetUpgradeLibrary.Services;
public class NoCustomProcessesHandler : IPostUpgradeProcessHandler, IPreUpgradeProcessHandler
{
    bool IPostUpgradeProcessHandler.ArePostUpgradeProcessesNeeded(DotNetUpgradeBasicConfig dotNetVersion)
    {
        dotNetVersion.CheckForTesting();
        return false;  // No post-upgrade processes needed
    }
    Task<bool> IPostUpgradeProcessHandler.HandleCommitAsync(LibraryNetUpgradeModel netUpdateModel, DotNetUpgradeBasicConfig dotNetVersion)
    {
        // Return false when no post-upgrade processes are needed
        dotNetVersion.CheckForTesting();
        return Task.FromResult(false);  // No custom commit handling needed.
    }
    Task IPostUpgradeProcessHandler.InitAsync(DotNetUpgradeBasicConfig dotNetVersion)
    {
        dotNetVersion.CheckForTesting();
        return Task.CompletedTask;
    }
    Task<bool> IPostUpgradeProcessHandler.RunPostUpgradeProcessesAsync(DotNetUpgradeBasicConfig dotNetVersion)
    {
        dotNetVersion.CheckForTesting();
        // Throw an exception if post-upgrade processes are triggered unexpectedly
        throw new InvalidOperationException("Post-upgrade processes were called, but none are configured. This should not happen.");
    }
    Task IPreUpgradeProcessHandler.InitAsync(DotNetUpgradeBasicConfig dotNetVersion)
    {
        dotNetVersion.CheckForTesting();
        return Task.CompletedTask;
    }
    bool IPreUpgradeProcessHandler.ArePreUpgradeProcessesNeeded(DotNetUpgradeBasicConfig dotNetVersion)
    {
        dotNetVersion.CheckForTesting();
        return false;  // No pre-upgrade processes needed
    }
    Task<bool> IPreUpgradeProcessHandler.RunPreUpgradeProcessesAsync(DotNetUpgradeBasicConfig dotNetVersion)
    {
        dotNetVersion.CheckForTesting();
        // Throw an exception if pre-upgrade processes are triggered unexpectedly
        throw new InvalidOperationException("Pre-upgrade processes were called, but none are configured. This should not happen.");
    }
    Task IPreUpgradeProcessHandler.ResetFlagsForNewVersionAsync(DotNetUpgradeBasicConfig dotNetVersion)
    {
        dotNetVersion.CheckForTesting();
        return Task.CompletedTask;  // No flags to reset for the new version
    }

    Task IPostUpgradeProcessHandler.ResetFlagsForNewVersionAsync(DotNetUpgradeBasicConfig dotNetVersion)
    {
        dotNetVersion.CheckForTesting();
        return Task.CompletedTask;  // No flags to reset for the new version
    }
}
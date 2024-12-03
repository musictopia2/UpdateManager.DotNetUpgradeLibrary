namespace UpdateManager.DotNetUpgradeLibrary.Services;
public class NoCustomProcessesHandler : IPostUpgradeProcessHandler, IPreUpgradeProcessHandler
{
    Task IPostUpgradeProcessHandler.ResetFlagsForNewVersionAsync(DotNetUpgradeConfigurationModel dotNetVersion)
    {
        dotNetVersion.CheckForTesting();
        return Task.CompletedTask;
    }
    bool IPostUpgradeProcessHandler.ArePostUpgradeProcessesNeeded(DotNetUpgradeConfigurationModel dotNetVersion)
    {
        dotNetVersion.CheckForTesting();
        return false;  // No post-upgrade processes needed
    }
    Task<bool> IPostUpgradeProcessHandler.HandleCommitAsync(LibraryNetUpgradeModel netUpdateModel, DotNetUpgradeConfigurationModel dotNetVersion)
    {
        // Return false when no post-upgrade processes are needed
        dotNetVersion.CheckForTesting();
        return Task.FromResult(false);  // No custom commit handling needed.
    }
    Task IPostUpgradeProcessHandler.InitAsync(DotNetUpgradeConfigurationModel dotNetVersion)
    {
        dotNetVersion.CheckForTesting();
        return Task.CompletedTask;
    }
    Task<bool> IPostUpgradeProcessHandler.RunPostUpgradeProcessesAsync(DotNetUpgradeConfigurationModel dotNetVersion)
    {
        dotNetVersion.CheckForTesting();
        // Throw an exception if post-upgrade processes are triggered unexpectedly
        throw new InvalidOperationException("Post-upgrade processes were called, but none are configured. This should not happen.");
    }
    Task IPreUpgradeProcessHandler.InitAsync(DotNetUpgradeConfigurationModel dotNetVersion)
    {
        dotNetVersion.CheckForTesting();
        return Task.CompletedTask;
    }
    bool IPreUpgradeProcessHandler.ArePreUpgradeProcessesNeeded(DotNetUpgradeConfigurationModel dotNetVersion)
    {
        dotNetVersion.CheckForTesting();
        return false;  // No pre-upgrade processes needed
    }
    Task<bool> IPreUpgradeProcessHandler.RunPreUpgradeProcessesAsync(DotNetUpgradeConfigurationModel dotNetVersion)
    {
        dotNetVersion.CheckForTesting();
        // Throw an exception if pre-upgrade processes are triggered unexpectedly
        throw new InvalidOperationException("Pre-upgrade processes were called, but none are configured. This should not happen.");
    }
    Task IPreUpgradeProcessHandler.ResetFlagsForNewVersionAsync(DotNetUpgradeConfigurationModel dotNetVersion)
    {
        dotNetVersion.CheckForTesting();
        return Task.CompletedTask;  // No flags to reset for the new version
    }
}
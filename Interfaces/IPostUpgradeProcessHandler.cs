namespace UpdateManager.DotNetUpgradeLibrary.Interfaces;
public interface IPostUpgradeProcessHandler
{
    // Asynchronous method to initialize any necessary data or resources
    Task InitAsync(DotNetUpgradeBasicConfig dotNetVersion);

    // Determines if any custom processes are needed
    bool ArePostUpgradeProcessesNeeded(DotNetUpgradeBasicConfig dotNetVersion);

    // Resets any flags or state for a new version (this could be necessary to reset before the pre-upgrade starts)
    Task ResetFlagsForNewVersionAsync(DotNetUpgradeBasicConfig dotNetVersion);
    // Executes custom processes and returns a bool indicating success
    Task<bool> RunPostUpgradeProcessesAsync(DotNetUpgradeBasicConfig dotNetVersion);

    Task<bool> HandleCommitAsync(LibraryNetUpgradeModel netUpdateModel, DotNetUpgradeBasicConfig dotNetVersion);
}
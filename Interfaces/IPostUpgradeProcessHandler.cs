namespace UpdateManager.DotNetUpgradeLibrary.Interfaces;
public interface IPostUpgradeProcessHandler
{
    // Asynchronous method to initialize any necessary data or resources
    Task InitAsync(DotNetUpgradeConfigurationModel dotNetVersion);

    // Determines if any custom processes are needed
    bool ArePostUpgradeProcessesNeeded(DotNetUpgradeConfigurationModel dotNetVersion);

    // Resets any flags or state for a new version
    Task ResetFlagsForNewVersionAsync(DotNetUpgradeConfigurationModel dotNetVersion);

    // Executes custom processes and returns a bool indicating success
    Task<bool> RunPostUpgradeProcessesAsync(DotNetUpgradeConfigurationModel dotNetVersion);

    Task<bool> HandleCommitAsync(LibraryNetUpgradeModel netUpdateModel, DotNetUpgradeConfigurationModel dotNetVersion);
}
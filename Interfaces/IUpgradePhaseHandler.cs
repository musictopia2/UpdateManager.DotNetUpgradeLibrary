namespace UpdateManager.DotNetUpgradeLibrary.Interfaces;
public interface IUpgradePhaseHandler
{
    // Asynchronous method to initialize any necessary data or resources
    Task InitAsync();

    // Determines if any custom processes are needed
    bool ArePostUpgradeProcessesNeeded();

    // Resets any flags or state for a new version (this could be necessary to reset before the pre-upgrade starts)
    Task ResetFlagsForNewVersionAsync();
    // Executes custom processes and returns a bool indicating success
    Task<bool> RunPostUpgradeProcessesAsync();
    // Determines if any pre-upgrade processes are needed
    bool ArePreUpgradeProcessesNeeded();

    // Executes the pre-upgrade processes (like updating the program to .NET 10 without modifying dependencies)
    Task<bool> RunPreUpgradeProcessesAsync();
    Task<bool> HandleCommitAsync(LibraryNetUpgradeModel netUpdateModel)
    {
        return Task.FromResult(false); //most of the time, this won't handle it.
    }
    string Name => "None";
}
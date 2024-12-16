namespace UpdateManager.DotNetUpgradeLibrary.Interfaces;
public interface IUpgradePhaseHandler
{
    // Asynchronous method to initialize any necessary data or resources
    Task InitAsync();

    // Resets any flags or state for a new version (this could be necessary to reset before the pre-upgrade starts)
    Task ResetFlagsForNewVersionAsync();

    // Determines if any custom processes are needed after the upgrade (PostUpgrade logic)
    bool ArePostUpgradeProcessesNeeded();

    // Determines if any pre-upgrade processes are needed (PreUpgrade logic)
    bool Are1PreUpgradeProcessesNeeded(); // NOTE: '1' prefix ensures PreUpgrade methods are listed first in Visual Studio.

    // Executes the pre-upgrade processes (like updating the program to .NET 10 without modifying dependencies)
    Task<bool> Run1PreUpgradeProcessesAsync(); // NOTE: '1' prefix ensures PreUpgrade methods come before PostUpgrade in alphabetical order.

    // Executes the post-upgrade processes and returns a bool indicating success
    Task<bool> RunPostUpgradeProcessesAsync();

    // A helper method to handle commits after the upgrade
    Task<bool> HandleCommitAsync(LibraryNetUpgradeModel netUpdateModel)
    {
        return Task.FromResult(false); // Most of the time, this won't handle it, but it's available if needed.
    }

    // The default name for the handler (it could be customized in implementations if necessary)
    string Name => "None";
}
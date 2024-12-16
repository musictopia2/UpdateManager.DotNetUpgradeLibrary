namespace UpdateManager.DotNetUpgradeLibrary.Interfaces;
public interface IUpgradePhaseHandler
{
    // Asynchronous method to initialize any necessary data or resources.
    // This could involve preparing configuration, setting up flags, or preparing the environment.
    Task InitAsync();

    // Resets any flags or state for a new version.
    // This should be called before the pre-upgrade begins to ensure that any previous state doesn't interfere.
    Task ResetFlagsForNewVersionAsync();

    // Determines if any custom processes are needed after the upgrade (PostUpgrade logic).
    // Returns true if any post-upgrade actions need to be taken; false if no actions are necessary.
    bool ArePostUpgradeProcessesNeeded();

    // Determines if any pre-upgrade processes are needed (PreUpgrade logic).
    // Returns true if pre-upgrade actions need to be performed, or false if not.
    // NOTE: The '1' prefix ensures that PreUpgrade methods are listed before PostUpgrade methods 
    // in Visual Studio's alphabetical method listing, to avoid confusion between the two.
    bool Are1PreUpgradeProcessesNeeded();

    // Executes the pre-upgrade processes (like updating the program to .NET 10 without modifying dependencies).
    // This method handles all logic required before the upgrade takes place.
    // Returns true if the pre-upgrade processes complete successfully, false otherwise.
    // NOTE: The '1' prefix ensures that PreUpgrade methods are listed before PostUpgrade methods 
    // in Visual Studio's alphabetical method listing, ensuring the PreUpgrade steps are clearly seen first.
    Task<bool> Run1PreUpgradeProcessesAsync();

    // Executes the post-upgrade processes, which typically involve tasks that occur after the upgrade completes,
    // such as updating dependencies or finalizing changes.
    // Returns true if the post-upgrade processes complete successfully, false otherwise.
    Task<bool> RunPostUpgradeProcessesAsync();

    // A helper method to handle commits after the upgrade.
    // Most of the time, this won't handle it, but it provides a default implementation that returns false.
    // Can be overridden in derived classes if custom commit logic is required.
    Task<bool> HandleCommitAsync(LibraryNetUpgradeModel netUpdateModel)
    {
        return Task.FromResult(false); // By default, this method does not handle commits.
    }

    // The default name for the handler.
    // This is provided as a read-only property, and could be customized in derived implementations if needed.
    string Name => "None";
}
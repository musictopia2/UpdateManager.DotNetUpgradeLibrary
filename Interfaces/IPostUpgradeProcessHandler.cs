namespace UpdateManager.DotNetUpgradeLibrary.Interfaces;
public interface IPostUpgradeProcessHandler
{
    // Asynchronous method to initialize any necessary data or resources
    Task InitAsync(DotNetUpgradeBasicConfig dotNetVersion);

    // Determines if any custom processes are needed
    bool ArePostUpgradeProcessesNeeded(DotNetUpgradeBasicConfig dotNetVersion);


    // Executes custom processes and returns a bool indicating success
    Task<bool> RunPostUpgradeProcessesAsync(DotNetUpgradeBasicConfig dotNetVersion);

    Task<bool> HandleCommitAsync(LibraryNetUpgradeModel netUpdateModel, DotNetUpgradeBasicConfig dotNetVersion);
}
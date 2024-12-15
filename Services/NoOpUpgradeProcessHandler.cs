namespace UpdateManager.DotNetUpgradeLibrary.Services;
public class NoOpUpgradeProcessHandler : IUpgradePhaseHandler
{

    public bool ArePostUpgradeProcessesNeeded() => false;
    public bool ArePreUpgradeProcessesNeeded() => false;
    public Task<bool> HandleCommitAsync(LibraryNetUpgradeModel netUpdateModel) => Task.FromResult(false);
    public Task InitAsync() => Task.CompletedTask;
    public Task ResetFlagsForNewVersionAsync() => Task.CompletedTask;
    public Task<bool> RunPostUpgradeProcessesAsync() => Task.FromResult(true);
    public Task<bool> RunPreUpgradeProcessesAsync() => Task.FromResult(true);
}
namespace UpdateManager.DotNetUpgradeLibrary.Services;
public class NoOpUpgradeProcessHandler : IUpgradePhaseHandler
{

    public bool ArePostUpgradeProcessesNeeded() => false;
    public bool Are1PreUpgradeProcessesNeeded() => false;
    public Task<bool> HandleCommitAsync(LibraryNetUpgradeModel netUpdateModel) => Task.FromResult(false);
    public Task InitAsync() => Task.CompletedTask;
    public Task ResetFlagsForNewVersionAsync() => Task.CompletedTask;
    public Task<bool> RunPostUpgradeProcessesAsync() => Task.FromResult(true);
    public Task<bool> Run1PreUpgradeProcessesAsync() => Task.FromResult(true);
}
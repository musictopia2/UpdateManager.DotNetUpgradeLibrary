namespace UpdateManager.DotNetUpgradeLibrary.Interfaces;
public interface INoPreUpgradeProcessing : IUpgradePhaseHandler
{
    bool IUpgradePhaseHandler.Are1PreUpgradeProcessesNeeded() => false;
    Task<bool> IUpgradePhaseHandler.Run1PreUpgradeProcessesAsync() => throw new CustomBasicException("No pre processes needed");
}
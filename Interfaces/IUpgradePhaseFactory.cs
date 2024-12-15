namespace UpdateManager.DotNetUpgradeLibrary.Interfaces;
public interface IUpgradePhaseFactory
{
    BasicList<IUpgradePhaseHandler> CreateUpgradePhases { get; }
}
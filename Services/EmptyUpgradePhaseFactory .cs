namespace UpdateManager.DotNetUpgradeLibrary.Services;
public class EmptyUpgradePhaseFactory : IUpgradePhaseFactory
{
    BasicList<IUpgradePhaseHandler> IUpgradePhaseFactory.CreateUpgradePhases => [];
}
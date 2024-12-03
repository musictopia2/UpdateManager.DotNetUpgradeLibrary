namespace UpdateManager.DotNetUpgradeLibrary.Interfaces;
public interface IFeedPathResolver
{
    string GetFeedPath(LibraryNetUpgradeModel upgradeModel, DotNetUpgradeConfigurationModel netModel);
}
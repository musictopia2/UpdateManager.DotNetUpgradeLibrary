namespace UpdateManager.DotNetUpgradeLibrary.Interfaces;
public interface IFeedPathResolver
{
    string GetFeedPath(LibraryNetUpgradeModel upgradeModel, DotNetUpgradeBasicConfig netModel);
}
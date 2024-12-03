namespace UpdateManager.DotNetUpgradeLibrary.Interfaces;
public interface IFeedPathResolver
{
    string GetFeedPath(LibraryNetUpdateModel upgradeModel, DotNetVersionUpgradeModel netModel);
}
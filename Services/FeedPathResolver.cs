namespace UpdateManager.DotNetUpgradeLibrary.Services;
public class FeedPathResolver(IPostBuildCommandStrategy postBuildCommand) : IFeedPathResolver
{
    string IFeedPathResolver.GetFeedPath(LibraryNetUpgradeModel upgradeModel)
    {
        if (postBuildCommand.ShouldRunPostBuildCommand(upgradeModel))
        {
            return ""; //because the post program will handle this  i think if this is development, the program is going to know about it anyways.  let it handle this.
        }
        // Handle Development
        if (upgradeModel.Development)
        {
            return bb1.Configuration!.DevelopmentPackagePath;
        }
        // Handle Local or Staging Feeds
        return upgradeModel.PackageType == EnumFeedType.Local
            ? bb1.Configuration!.PrivatePackagePath
            : bb1.Configuration!.StagingPackagePath;
    }
}
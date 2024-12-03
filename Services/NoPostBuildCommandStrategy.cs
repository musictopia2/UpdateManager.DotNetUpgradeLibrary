namespace UpdateManager.DotNetUpgradeLibrary.Services;
public class NoPostBuildCommandStrategy : IPostBuildCommandStrategy
{
    bool IPostBuildCommandStrategy.ShouldRunPostBuildCommand(LibraryNetUpdateModel libraryModel)
    {
        return false;
    }
}
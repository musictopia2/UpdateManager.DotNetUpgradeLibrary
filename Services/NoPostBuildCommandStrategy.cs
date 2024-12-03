namespace UpdateManager.DotNetUpgradeLibrary.Services;
public class NoPostBuildCommandStrategy : IPostBuildCommandStrategy
{
    bool IPostBuildCommandStrategy.ShouldRunPostBuildCommand(LibraryNetUpgradeModel libraryModel)
    {
        return false;
    }
}
namespace UpdateManager.DotNetUpgradeLibrary.Interfaces;
public interface IPostBuildCommandStrategy
{
    bool ShouldRunPostBuildCommand(LibraryNetUpgradeModel libraryModel);
}
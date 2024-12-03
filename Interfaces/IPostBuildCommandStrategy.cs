namespace UpdateManager.DotNetUpgradeLibrary.Interfaces;
public interface IPostBuildCommandStrategy
{
    bool ShouldRunPostBuildCommand(LibraryNetUpdateModel libraryModel);
}
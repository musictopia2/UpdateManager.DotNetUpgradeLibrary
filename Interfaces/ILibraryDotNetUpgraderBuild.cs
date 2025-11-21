namespace UpdateManager.DotNetUpgradeLibrary.Interfaces;
public interface ILibraryDotNetUpgraderBuild
{
    //if this has already been built, then can mark as complete.
    Task<bool> BuildLibraryAsync(LibraryNetUpgradeModel libraryModel, BasicList<LibraryNetUpgradeModel> libraries, CancellationToken cancellationToken = default);
}
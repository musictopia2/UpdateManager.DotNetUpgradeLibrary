namespace UpdateManager.DotNetUpgradeLibrary.Interfaces;
public interface ILibraryDotNetUpgraderBuild
{
    //if this has already been built, then can mark as complete.
    Task<bool> AlreadyUpgradedAsync(LibraryNetUpgradeModel upgradeModel, DotNetUpgradeConfigurationModel dotNetModel);
    Task<bool> BuildLibraryAsync(LibraryNetUpgradeModel libraryModel, DotNetUpgradeConfigurationModel dotNetModel, BasicList<LibraryNetUpgradeModel> libraries, CancellationToken cancellationToken = default);
}
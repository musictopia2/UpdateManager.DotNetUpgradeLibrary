namespace UpdateManager.DotNetUpgradeLibrary.Interfaces;
public interface INetVersionUpdateContext
{
    Task<BasicList<LibraryNetUpgradeModel>> GetLibrariesForUpdateAsync();
    Task SaveUpdatedLibrariesAsync(BasicList<LibraryNetUpgradeModel> list);
    Task ResetLibraryAsync(); //this means you need to delete the library since it has to be repopulated
    bool IsLibraryDataPresent();
    Task<BasicList<LibraryNetUpgradeModel>> ReprocessLibrariesForUpdateAsync(); //if you have to update the .net then needs to redo the libraries
}
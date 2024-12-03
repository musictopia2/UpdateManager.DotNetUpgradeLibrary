namespace UpdateManager.DotNetUpgradeLibrary.Interfaces;
public interface ILibraryNetUpdateModelGenerator
{
    Task<BasicList<LibraryNetUpgradeModel>> CreateLibraryNetUpdateModelListAsync();
}
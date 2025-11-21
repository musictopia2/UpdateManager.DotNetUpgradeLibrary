namespace UpdateManager.DotNetUpgradeLibrary.Services;
public class FileNetVersionUpdateContext(ILibraryNetUpdateModelGenerator generator) : INetVersionUpdateContext
{
    private static readonly string _libraryPath = bb1.Configuration!.LibraryPath;
    async Task<BasicList<LibraryNetUpgradeModel>> INetVersionUpdateContext.GetLibrariesForUpdateAsync()
    {
        BasicList<LibraryNetUpgradeModel> list = await jj1.RetrieveSavedObjectAsync<BasicList<LibraryNetUpgradeModel>>(_libraryPath);
        return list;
    }
    bool INetVersionUpdateContext.IsLibraryDataPresent()
    {
        return ff1.FileExists(_libraryPath);
    }
    async Task<BasicList<LibraryNetUpgradeModel>> INetVersionUpdateContext.ReprocessLibrariesForUpdateAsync()
    {
        BasicList<LibraryNetUpgradeModel> output = await generator.CreateLibraryNetUpdateModelListAsync();
        await jj1.SaveObjectAsync(_libraryPath, output);
        return output;
    }
    async Task INetVersionUpdateContext.ResetLibraryAsync()
    {
        await ff1.DeleteFileAsync(_libraryPath);
    }
    async Task INetVersionUpdateContext.SaveUpdatedLibrariesAsync(BasicList<LibraryNetUpgradeModel> list)
    {
        await jj1.SaveObjectAsync(_libraryPath, list);
    }
}
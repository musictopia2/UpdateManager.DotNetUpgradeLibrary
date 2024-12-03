namespace UpdateManager.DotNetUpgradeLibrary.Interfaces;
public interface IDotNetVersionInfoRepository
{
    Task<DotNetVersionUpgradeModel> GetVersionInfoAsync();
    Task SaveVersionInfoAsync(DotNetVersionUpgradeModel model);
}
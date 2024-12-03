namespace UpdateManager.DotNetUpgradeLibrary.Interfaces;
public interface IDotNetVersionInfoRepository
{
    Task<DotNetUpgradeConfigurationModel> GetVersionInfoAsync();
    Task SaveVersionInfoAsync(DotNetUpgradeConfigurationModel model);
}
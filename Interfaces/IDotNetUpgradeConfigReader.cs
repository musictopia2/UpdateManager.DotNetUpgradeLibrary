namespace UpdateManager.DotNetUpgradeLibrary.Interfaces;
public interface IDotNetUpgradeConfigReader
{
    Task<DotNetUpgradeBasicConfig> GetConfigAsync();
}
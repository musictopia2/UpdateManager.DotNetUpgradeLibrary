namespace UpdateManager.DotNetUpgradeLibrary.Services;
public class DotNetUpgradeConfigReader : IDotNetUpgradeConfigReader
{
    Task<DotNetUpgradeBasicConfig> IDotNetUpgradeConfigReader.GetConfigAsync()
    {
        // Logic to read and return the upgrade configuration
        var config = new DotNetUpgradeBasicConfig
        {
            IsTestMode = bb1.Configuration!.GetValue<bool>(nameof(DotNetUpgradeBasicConfig.IsTestMode)),
            TestLocalFeedPath = bb1.Configuration!.GetValue<string>(nameof(DotNetUpgradeBasicConfig.TestLocalFeedPath))!,
            TestPublicFeedPath = bb1.Configuration!.GetValue<string>(nameof(DotNetUpgradeBasicConfig.TestPublicFeedPath))!
        };
        if (string.IsNullOrEmpty(config.TestLocalFeedPath) || string.IsNullOrEmpty(config.TestPublicFeedPath))
        {
            throw new InvalidOperationException("Test feed paths must be configured.");
        }
        return Task.FromResult(config);
    }
}
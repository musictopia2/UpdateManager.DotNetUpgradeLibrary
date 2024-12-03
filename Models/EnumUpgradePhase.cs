namespace UpdateManager.DotNetUpgradeLibrary.Models;
public enum EnumUpgradePhase
{
    PreUpgradeCompleted,    // Pre-upgrade processes are completed, waiting for the main upgrade
    PreUpgradePending,      // Pre-upgrade processes are required and pending
    PendingUpdate,          // Upgrade to the new .NET version is required (main upgrade process)
    UpgradeInProgress,      // Main upgrade is running (excluding pre-upgrade tasks)
    UpgradeCompleted,       // Main upgrade is completed; post-processing tasks may still need to run
}
namespace UpdateManager.DotNetUpgradeLibrary.Models;
public enum EnumUpgradeStatus
{
    None,              // No upgrade needed or no pre-upgrade process is applicable
    PreProcessed,      // Pre-upgrade processes are complete (e.g., .NET version and dependencies updated)
    CheckedOut,        // Repository's default branch has been successfully checked out (or no repo)
    Built,             // Project successfully built (post-upgrade steps may still be pending)
    Completed          // Full upgrade (pre and post) process is completed
}
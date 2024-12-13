namespace UpdateManager.DotNetUpgradeLibrary.Models;
public enum EnumDotNetUpgradeStatus
{
    None,                // Waiting to be processed
    BranchCheckedOut,    // Main branch has been checked out (or is already on it)
    Built,               // Successfully built
    Published,           // Successfully published to local feed
    //i don't think there is a need as far as i know to publish on nuget (if i am wrong, rethink)
    WaitingForGitHub,    // Published on NuGet but waiting to commit on GitHub
    Completed            // All steps completed (including GitHub commit)
}
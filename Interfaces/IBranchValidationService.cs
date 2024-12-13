namespace UpdateManager.DotNetUpgradeLibrary.Interfaces;
public interface IBranchValidationService
{
    // Ensures the library is on the correct branch (e.g., "main") before continuing with the upgrade.
    Task<bool> ValidateBranchAsync(LibraryNetUpgradeModel updateModel, DotNetUpgradeBasicConfig versionUpgradeModel, CancellationToken cancellationToken = default);
}
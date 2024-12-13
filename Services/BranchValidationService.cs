namespace UpdateManager.DotNetUpgradeLibrary.Services;
public class BranchValidationService(IPostUpgradeProcessHandler handler) : IBranchValidationService
{
    async Task<bool> IBranchValidationService.ValidateBranchAsync(LibraryNetUpgradeModel updateModel, DotNetUpgradeBasicConfig versionUpgradeModel, CancellationToken cancellationToken)
    {
        if (await handler.HandleCommitAsync(updateModel, versionUpgradeModel))
        {
            return true; //something else should have handled this earlier
        }
        bool switchResult;
        string? directory = Path.GetDirectoryName(updateModel.CsProjPath);

        if (string.IsNullOrEmpty(directory))
        {
            throw new InvalidOperationException($"The project path for {updateModel.PackageName} is invalid or null.");
        }
        switchResult = await GitBranchManager.SwitchBranchAsync("Main", directory, cancellationToken);
        return switchResult;
    }
}
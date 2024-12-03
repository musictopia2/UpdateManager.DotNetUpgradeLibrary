﻿namespace UpdateManager.DotNetUpgradeLibrary.Services;
public class LibraryDotNetUpgraderBuild(IPostBuildCommandStrategy postBuildStrategy) : ILibraryDotNetUpgraderBuild
{
    async Task<bool> ILibraryDotNetUpgraderBuild.BuildLibraryAsync(LibraryNetUpgradeModel libraryModel, DotNetUpgradeConfigurationModel dotNetModel, BasicList<LibraryNetUpgradeModel> libraries, CancellationToken cancellationToken)
    {
        CsProjEditor editor = new(libraryModel.CsProjPath);
        string netVersion = bb1.Configuration!.GetNetVersion();
        bool isSuccess;
        isSuccess = editor.UpdateNetVersion(netVersion);
        if (isSuccess == false)
        {
            return false;
        }
        isSuccess = await editor.UpdateDependenciesAsync(libraries);
        if (isSuccess == false)
        {
            return false;
        }
        if (dotNetModel.IsTestMode == false)
        {
            if (postBuildStrategy.ShouldRunPostBuildCommand(libraryModel))
            {
                isSuccess = editor.UpdatePostBuildCommand(netVersion);
                if (isSuccess == false)
                {
                    return false;
                }
            }
        }
        isSuccess = await ProjectBuilder.BuildProjectAsync(libraryModel.CsProjPath, "/p:SkipPostBuild=true", cancellationToken);
        return isSuccess;
    }
    async Task<bool> ILibraryDotNetUpgraderBuild.AlreadyUpgradedAsync(LibraryNetUpgradeModel upgradeModel, DotNetUpgradeConfigurationModel dotNetModel)
    {
        return await upgradeModel.AlreadyUpgradedAsync(dotNetModel);
    }
}
﻿namespace UpdateManager.DotNetUpgradeLibrary.Interfaces;
public interface ILibraryDotNetUpgraderBuild
{
    //if this has already been built, then can mark as complete.
    Task<bool> AlreadyUpgradedAsync(LibraryNetUpdateModel upgradeModel, DotNetVersionUpgradeModel dotNetModel);
    Task<bool> BuildLibraryAsync(LibraryNetUpdateModel libraryModel, DotNetVersionUpgradeModel dotNetModel, BasicList<LibraryNetUpdateModel> libraries, CancellationToken cancellationToken = default);
}
﻿namespace UpdateManager.DotNetUpgradeLibrary.Extensions;
public static class CustomProcessesExtensions
{
    public static void CheckForTesting(this DotNetUpgradeBasicConfig model)
    {
        if (model.IsTestMode)
        {
            throw new CustomBasicException("Cannot run custom processes because its in test mode");
        }
    }
}
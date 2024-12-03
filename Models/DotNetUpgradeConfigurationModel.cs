﻿namespace UpdateManager.DotNetUpgradeLibrary.Models;
public class DotNetUpgradeConfigurationModel
{
    public int NetVersion { get; set; }
    public bool IsTestMode { get; set; }  // Determines if it's in test mode
    public string TestLocalFeedPath { get; set; } = "";  // Path to the test local feed
    public string TestPublicFeedPath { get; set; } = "";  // Path to the test public feed
    //because i will have staging, no longer necessary to have the other temporary feed anymore
}
﻿namespace UpdateManager.DotNetUpgradeLibrary.Models;
public readonly record struct UpgradeProcessState(EnumUpgradePhase Start, DotNetUpgradeBasicConfig NetUpgrade);
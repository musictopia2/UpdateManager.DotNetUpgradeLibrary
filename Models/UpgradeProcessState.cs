﻿namespace UpdateManager.DotNetUpgradeLibrary.Models;
public readonly record struct UpgradeProcessState(EnumUpgradePhase Start, DotNetVersionUpgradeModel NetUpgrade);
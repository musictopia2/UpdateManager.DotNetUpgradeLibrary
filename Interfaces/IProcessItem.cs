namespace UpdateManager.DotNetUpgradeLibrary.Interfaces;
public interface IProcessItem
{
    string HiddenPath { get; } //only via interface
    EnumUpgradeStatus UpgradeStatus { get; set; }
}
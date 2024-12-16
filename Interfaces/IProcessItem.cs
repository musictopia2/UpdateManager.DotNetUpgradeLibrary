namespace UpdateManager.DotNetUpgradeLibrary.Interfaces;
public interface IProcessItem
{
    string HiddenPath { get; } //only via interface
    public EnumUpgradeStatus UpgradeStatus { get; set; }
}
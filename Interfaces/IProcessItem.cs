namespace UpdateManager.DotNetUpgradeLibrary.Interfaces;
public interface IProcessItem
{
    string Identifier { get; }
    public EnumUpgradeStatus UpgradeStatus { get; set; }
}
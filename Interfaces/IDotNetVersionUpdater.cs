namespace UpdateManager.DotNetUpgradeLibrary.Interfaces;
public interface IDotNetVersionUpdater
{
    Task UpdateNetVersionAsync(int newNetVersion);
}
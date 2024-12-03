namespace UpdateManager.DotNetUpgradeLibrary.Services;
public class TemplateNetUpdater() : ITemplateNetUpdater
{
    async Task<bool> ITemplateNetUpdater.UpgradeTemplateAsync<T>(TemplateModel template, BasicList<T> libraries)
    {
        CsProjEditor editor = new(template.CsProjFile);
        bool rets;
        string version = bb1.Configuration!.GetNetPath();
        rets = editor.UpdateNetVersion(version);
        if (rets == false)
        {
            return false;
        }
        rets = await editor.UpdateDependenciesAsync(libraries);
        if (rets == false)
        {
            return false;
        }
        rets = await TemplateActionHandler.RunTemplateActionAsync(template.TemplateDirectory, EnumTemplateAction.Uninstall);
        if (rets == false)
        {
            return false;
        }
        rets = await TemplateActionHandler.RunTemplateActionAsync(version, EnumTemplateAction.Install);
        return rets;
    }
}
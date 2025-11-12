namespace UpdateManager.DotNetUpgradeLibrary.Utilities;
public static class DotNetProjectUpgradeHelper
{
    public static async Task<bool> UpgradeProjectToLatestDotNetAsync<T>(string projectFilePath, BasicList<T> packages, bool alsoPostProgram = false)
        where T: class, INugetModel
    {
        string version = bb1.Configuration!.GetNetVersion();
        CsProjEditor editor = new(projectFilePath);
        bool rets;
        rets = editor.UpdateNetVersion(version);
        if (rets == false)
        {
            Console.WriteLine($"Failed to update the csproj file to latest .net for {projectFilePath}");
            return false;
        }
        rets = await editor.UpdateDependenciesAsync(packages);
        if (rets == false)
        {
            Console.WriteLine($"Failed to update the dependencies for {projectFilePath}");
            return false;
        }
        if (alsoPostProgram)
        {
            rets = editor.UpdatePostBuildCommand(version);
            if (rets == false)
            {
                Console.WriteLine($"Failed to update the post program for {projectFilePath}");
                return false;
            }
        }
        editor.SaveChanges();
        rets = await ProjectBuilder.BuildProjectAsync(projectFilePath);
        if (rets == false)
        {
            Console.WriteLine($"Failed to build project for {projectFilePath}");
            return false;
        }
        string directory = Path.GetDirectoryName(projectFilePath)!;
        rets = DotNetVersionHelper.IsExpectedVersionInReleaseBuild(directory);
        if (rets == false)
        {
            Console.WriteLine("Somehow failed to update to the latest .net  check the csproj files");
            return false;
        }
        return true;
    }
}
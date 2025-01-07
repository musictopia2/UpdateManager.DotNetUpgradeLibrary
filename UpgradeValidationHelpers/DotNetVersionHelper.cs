namespace UpdateManager.DotNetUpgradeLibrary.UpgradeValidationHelpers;
public class DotNetVersionHelper
{
    public static bool IsExpectedVersionInReleaseBuild(string projectDirectory)
    {
        // Ensure the project directory exists
        if (string.IsNullOrWhiteSpace(projectDirectory) || !Directory.Exists(projectDirectory))
        {
            throw new DirectoryNotFoundException($"Project directory not found: {projectDirectory}");
        }
        // Get the project name based on the .csproj file (not the directory name)
        string projectName = GetProjectNameFromDirectory(projectDirectory);

        // Define the path to the 'bin\Release' directory under the project
        string releaseBinDirectory = Path.Combine(projectDirectory, "bin", "Release");
        if (!Directory.Exists(releaseBinDirectory))
        {
            return false;  // No 'Release' directory found, indicating the build has not been done in Release mode
        }

        // Get all subdirectories under 'bin\Release'
        var buildConfigurations = Directory.GetDirectories(releaseBinDirectory);

        // Filter for directories that contain a 'net' version (e.g., 'net6.0', 'net7.0', etc.)
        var netDirectories = buildConfigurations
            .Where(d => d.Contains("net") && Directory.Exists(d))
            .Select(d => new DirectoryInfo(d))
            .ToList();

        // If no directories are found, return false
        if (netDirectories.Count == 0)
        {
            return false;
        }

        // Find the directory with the highest .NET version (e.g., 'net10.0' > 'net9.0')
        var latestNetDirectory = netDirectories
            .OrderByDescending(d => GetNetMajorVersionFromString(d.Name))
            .FirstOrDefault();

        if (latestNetDirectory == null)
        {
            return false;  // No valid .NET folder found
        }

        // Determine whether it's WebAssembly or Server-side
        bool isWebAssembly = projectDirectory.Contains("WebAssembly", StringComparison.OrdinalIgnoreCase);
        bool isServerSide = projectDirectory.Contains("Server", StringComparison.OrdinalIgnoreCase);

        // If it's purely WebAssembly and we're checking a WebAssembly client
        if (isWebAssembly && !isServerSide)
        {
            throw new InvalidOperationException("WebAssembly apps are not supported in this context.");
        }
        BasicList<string> files = ff1.FileList(latestNetDirectory.FullName);
        if (files.Count == 0)
        {
            return false; //if there are no files there, then still failed.
        }
        // Determine the correct file to check based on whether this is a WebAssembly or Server-side app
        string depsFile = GetDepsFileBasedOnProjectType(latestNetDirectory.FullName, projectName);
        string runtimeConfigFile = GetRuntimeConfigFileBasedOnProjectType(latestNetDirectory.FullName, projectName);

        // If neither file exists, return false
        if (string.IsNullOrWhiteSpace(depsFile) && string.IsNullOrWhiteSpace(runtimeConfigFile))
        {
            return false; // Neither .deps.json nor runtimeconfig.json found
        }

        // Now, we need to validate the version in .deps.json
        string expectedVersion = bb1.Configuration!.GetNetVersion();
        HtmlParser parses = new();
        string searches;
        if (File.Exists(depsFile))
        {
            parses.Body = ff1.AllText(depsFile);
            searches = $".NETCoreApp,Version=v{expectedVersion}";
            if (parses.DoesExist(searches))
            {
                return true;
            }
            return false;
        }
        if (File.Exists(runtimeConfigFile) == false)
        {
            return true; //may have no way to detect.  just return true.  if wrong, too bad.
        }


        parses.Body = ff1.AllText(runtimeConfigFile);
        searches = $"net{expectedVersion}.0";

        //searches = $".NETCoreApp,Version=v{expectedVersion}";
        if (parses.DoesExist(searches))
        {
            return true;
        }
        return false;
    }
    // Helper function to determine the appropriate runtimeconfig.json file based on the project type
    private static string GetRuntimeConfigFileBasedOnProjectType(string netDirectory, string projectName)
    {
        return Path.Combine(netDirectory, $"{projectName}.runtimeconfig.json");
    }

    // Helper function to determine the project name from the directory (get it from the .csproj file)
    private static string GetProjectNameFromDirectory(string projectDirectory)
    {
        var csprojFiles = Directory.GetFiles(projectDirectory, "*.csproj");
        if (csprojFiles.Length == 0)
        {
            throw new FileNotFoundException($"No .csproj file found in the directory: {projectDirectory}");
        }

        // Extract the project name from the csproj file name (without extension)
        string csprojFileName = Path.GetFileNameWithoutExtension(csprojFiles[0]);
        return csprojFileName;  // This should return the correct project name
    }

    // Helper function to determine the appropriate .deps.json file based on the project type
    private static string GetDepsFileBasedOnProjectType(string netDirectory, string projectName)
    {
        return Path.Combine(netDirectory, $"{projectName}.deps.json");
    }

    // Helper method to parse and compare the major .NET version from the directory name (e.g., 'net6.0' -> '6')
    private static int GetNetMajorVersionFromString(string directoryName)
    {
        // Extract the version part (e.g., 'net6.0' -> '6.0')
        var versionString = directoryName.Substring(directoryName.IndexOf("net") + 3);

        // Ensure that we handle cases where version might have extra digits or an additional suffix
        versionString = versionString.Split('-').First(); // Remove any additional suffix like 'windows' or 'linux'

        // Parse and return the major version (e.g., '6.0' -> 6)
        var versionParts = versionString.Split('.');
        if (int.TryParse(versionParts[0], out int majorVersion))
        {
            return majorVersion;
        }
        // Default to 0 if parsing fails
        return 0;
    }
}
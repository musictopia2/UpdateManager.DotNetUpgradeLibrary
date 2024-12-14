namespace UpdateManager.DotNetUpgradeLibrary.UpgradeValidationHelpers;
public class DotNetVersionHelper
{
    // Method to verify if the expected .NET version exists in the Release build output
    public static bool IsExpectedVersionInReleaseBuild(string projectDirectory)
    {
        // Ensure the project directory exists
        if (string.IsNullOrWhiteSpace(projectDirectory) || !Directory.Exists(projectDirectory))
        {
            throw new DirectoryNotFoundException($"Project directory not found: {projectDirectory}");
        }

        // Get the project name from the directory (last part of the path)
        string projectName = new DirectoryInfo(projectDirectory).Name;

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

        // Now, we need to check for the runtimeconfig.json file in the selected directory
        var runtimeConfigFile = Path.Combine(latestNetDirectory.FullName, $"{projectName}.runtimeconfig.json");
        //CommonBasicLibraries.deps.json
        var otherFile = Path.Combine(latestNetDirectory.FullName, $"{projectName}.deps.json");

        // If runtimeconfig.json doesn't exist, return false
        if (File.Exists(runtimeConfigFile) == false && File.Exists(otherFile)== false)
        {
            return false; // No runtimeconfig.json found
        }
        string expectedVersion = bb1.Configuration!.GetNetVersion();
        if (File.Exists(runtimeConfigFile) == false)
        {
            HtmlParser parses = new();
            parses.Body = ff1.AllText(otherFile);
            string searches = $".NETCoreApp,Version=v{expectedVersion}";
            return parses.DoesExist(searches);
        }

        // Read the runtimeconfig.json file to get the .NET version
        var runtimeConfig = ReadRuntimeConfig(runtimeConfigFile);
       
        // If runtimeconfig.json could not be read or the version is incorrect, return false
        if (runtimeConfig == null || !runtimeConfig.IsVersionCorrect(expectedVersion))
        {
            return false;  // Version mismatch or file read failure
        }

        // If everything matches, return true
        return true;
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

    // Method to read and parse the runtimeconfig.json file
    private static RuntimeConfig? ReadRuntimeConfig(string filePath)
    {
        try
        {
            string jsonString = File.ReadAllText(filePath);
            var runtimeConfig = SystemTextJsonStrings.DeserializeObject<RuntimeConfig>(jsonString);
            return runtimeConfig;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading {filePath}: {ex.Message}");
            return null;  // If reading fails, return null
        }
    }
}
namespace UpdateManager.DotNetUpgradeLibrary.Utilities;
public static class DotNetVersionChecker
{
    internal static bool IsDotNetVersionInstalled(int versionToCheck)
    {
        try
        {
            // Run the 'dotnet --list-runtimes' command to get the installed .NET runtimes
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "--list-runtimes",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processStartInfo);
            // Read the output (installed runtimes)
            string output = process!.StandardOutput.ReadToEnd();

            // Split by new lines and check each line for the version
            var installedVersions = output.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries)
                                          .Select(line => line.Trim())
                                          .ToBasicList();

            // Check if the desired version is present in the list
            string targetVersion = $"Microsoft.NETCore.App {versionToCheck}";
            return installedVersions.Any(version => version.Contains(targetVersion));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking .NET version: {ex.Message}");
            return false;
        }
    }
    public static int? GetHighestInstalledDotNetVersion()
    {
        try
        {
            // Execute 'dotnet --list-runtimes' command to get the installed runtimes
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "--list-runtimes",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var process = Process.Start(processStartInfo);
            string output = process!.StandardOutput.ReadToEnd();
            var installedVersions = output.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries)
                                          .Select(line => line.Trim())
                                          .Where(line => line.Contains("Microsoft.NETCore.App"))
                                          .Select(ExtractVersion)
                                          .Where(version => version.HasValue)
                                          .ToBasicList();
            if (installedVersions.Count != 0)
            {
                // Return the highest version found
                return installedVersions.Max();
            }
            else
            {
                Console.WriteLine("No .NET runtimes found.");
                return null; // No version found
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving .NET versions: {ex.Message}");
            return null;
        }
    }

    private static int? ExtractVersion(string runtimeLine)
    {
        // Extract the version from the runtime line
        var versionString = runtimeLine.Split(' ').LastOrDefault();
        if (versionString != null && int.TryParse(versionString.Split('.')[0], out int majorVersion))
        {
            return majorVersion;
        }
        return null;
    }
}
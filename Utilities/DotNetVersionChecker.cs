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

            var installedVersions = output
                .Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim())
                // Check for all Microsoft runtimes, not just NETCore
                .Where(line => line.StartsWith("Microsoft.") && line.Contains(' '))
                .Select(line =>
                {
                    // Line format: "Microsoft.NETCore.App 10.0.0 [C:\Path]"
                    var parts = line.Split(' ');
                    if (parts.Length < 2)
                    {
                        return (int?)null;
                    }

                    var versionStr = parts[1];
                    if (Version.TryParse(versionStr, out var v))
                    {
                        return v.Major;
                    }
                    return null;
                })
                .Where(v => v.HasValue)
                .ToBasicList();

            if (installedVersions.Count == 0)
            {
                Console.WriteLine("No .NET runtimes found.");
                return null;
            }

            return installedVersions.Max();
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
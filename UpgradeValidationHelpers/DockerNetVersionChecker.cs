namespace UpdateManager.DotNetUpgradeLibrary.UpgradeValidationHelpers;
public class DockerNetVersionChecker
{
    public static async Task<bool> IsDockerUpgradedAsync(string containerName)
    {
        // Assuming ProcessRunner is a helper that runs commands inside Docker containers
        var result = await RunAsync($"exec {containerName.ToLower()} printenv DOTNET_VERSION");
        string nexts = result.Trim();
        string versionitHas = ParseVersion(nexts);
        string versionExpected = bb1.Configuration!.GetNetVersion(); //needs to be on the latest version.
        return versionitHas == versionExpected;
    }
    private static string ParseVersion(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
        {
            return "";
        }

        var versionParts = version.Split('.');
        return versionParts.Length > 0 ? versionParts[0] : "";  // returns the major version, e.g., "9"
    }
    private static async Task<string> RunAsync(string command)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "docker",  // Use docker executable directly for running commands
            Arguments = command,  // Pass the Docker command as an argument
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,  // Don't use shell execution for redirecting output
            CreateNoWindow = true
        };

        try
        {
            using var process = Process.Start(startInfo);
            if (process == null)
                throw new InvalidOperationException("Failed to start process.");

            // Capture standard output and errors
            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            // Log the error if any
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine($"Error: {error}");
            }

            return output.Trim();  // Trim output to remove unwanted newlines/spaces
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error running process: {ex.Message}");
            return "";
        }
    }
}
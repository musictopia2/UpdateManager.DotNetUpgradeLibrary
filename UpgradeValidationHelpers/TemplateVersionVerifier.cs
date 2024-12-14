namespace UpdateManager.DotNetUpgradeLibrary.UpgradeValidationHelpers;
public class TemplateVersionVerifier
{
    // Create a temporary project using the template and check its .csproj file
    public static bool IsTemplateUsingCorrectVersion(string templateName)
    {
        // Create a temporary directory for the test project
        string tempProjectDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        string expectedVersion = bb1.Configuration!.GetNetVersion();
        try
        {
            // Step 1: Create the project using the template
            Console.WriteLine("Creating a temporary project using the template...");
            var createProcess = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"new {templateName} -o \"{tempProjectDirectory}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var createProcessResult = Process.Start(createProcess);
            createProcessResult!.WaitForExit();

            if (createProcessResult.ExitCode != 0)
            {
                Console.WriteLine("Failed to create project using the template.");
                return false;
            }

            // Step 2: Read the .csproj file from the generated project
            string csprojPath = Directory.GetFiles(tempProjectDirectory, "*.csproj", SearchOption.TopDirectoryOnly).FirstOrDefault()!;
            //no problem to repeat here.  the computer generated it anyways.

            if (csprojPath == null)
            {
                Console.WriteLine("No .csproj file found in the generated project.");
                return false;
            }

            // Step 3: Check the .csproj for the target framework version
            XDocument csproj = XDocument.Load(csprojPath);
            var targetFrameworkElement = csproj.Descendants().FirstOrDefault(e => e.Name.LocalName == "TargetFramework");

            if (targetFrameworkElement == null)
            {
                Console.WriteLine("No TargetFramework found in the .csproj.");
                return false;
            }

            string tfm = targetFrameworkElement.Value;
            string majorVersion = tfm.Split('.')[0].Replace("net", string.Empty);

            // Step 4: Verify the major version matches the expected version
            return majorVersion == expectedVersion;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
        finally
        {
            // Step 5: Clean up the temporary project directory
            if (Directory.Exists(tempProjectDirectory))
            {
                Directory.Delete(tempProjectDirectory, true);
                Console.WriteLine("Temporary project deleted.");
            }
        }
    }
}
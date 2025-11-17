namespace UpdateManager.DotNetUpgradeLibrary.Services;
public class PostUpgradeCoordinator(IUpgradePhaseFactory factory)
{
    public async Task RunPostProcessesAsync()
    {
        var list = factory.CreateUpgradePhases;
        foreach (var item in list)
        {
            await item.InitAsync();
        }
        if (list.All(x => x.ArePostUpgradeProcessesNeeded() == false))
        {
            Console.WriteLine("There was no post upgrades needed");
            return;
        }
        foreach (var phase in list)
        {
            await phase.InitAsync(); //try to initialize again anyways.  don't know if it will hurt or not (?)
            if (phase.ArePostUpgradeProcessesNeeded())
            {
                Console.WriteLine($"Running process for {phase.Name}");
                bool rets;
                rets = await phase.RunPostUpgradeProcessesAsync();
                if (rets == false)
                {
                    Console.WriteLine($"Failed upgrade for {phase.Name}.  Exiting");
                    return;
                }
            }
        }
    }
    public async Task ResetProcessesManuallyAsync()
    {
        var list = factory.CreateUpgradePhases;
        foreach (var item in list)
        {
            await item.ResetFlagsForNewVersionAsync();
        }
    }
}
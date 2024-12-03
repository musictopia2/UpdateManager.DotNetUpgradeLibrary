namespace UpdateManager.DotNetUpgradeLibrary.Extensions;
public static class ServiceExtensions
{
    public static IServiceCollection RegisterDotNetUpgradeServices(this IServiceCollection services, bool useFileBased = true, bool alsoBuilder = true, Action<IServiceCollection>? additionalServices = null)
    {
        services.AddSingleton<IDateOnlyPicker, RealDatePicker>()
            .AddSingleton<IFeedPathResolver, FeedPathResolver>()
            .AddSingleton<ITestFileManager, TestFileManager>()
            .AddSingleton<ITemplateNetUpdater, TemplateNetUpdater>()
            .AddSingleton<LibraryDotNetUpgradeCommitter, LibraryDotNetUpgradeCommitter>()
            .AddSingleton<ILibraryNetUpdateModelGenerator, LibraryNetUpdateModelGenerator>()
            .AddSingleton<IYearlyFeedManager, YearlyFeedManager>()
            .AddSingleton<DotNetUpgradeCoordinator>();
        if (alsoBuilder)
        {
            services.AddSingleton<ILibraryDotNetUpgraderBuild, LibraryDotNetUpgraderBuild>();
        }
        if (useFileBased)
        {
            services.AddSingleton<IDotNetVersionInfoRepository, FileDotNetVersionInfoManager>()
                .AddSingleton<INetVersionUpdateContext, FileNetVersionUpdateContext>()
                .AddSingleton<IPackagesContext, FilePackagesContext>();
        }
        if (additionalServices is not null)
        {
            additionalServices.Invoke(services);
        }
        return services;
    }
    public static IServiceCollection RegisterDefaultUpgradeHandlers(this IServiceCollection services)
    {
        services.AddSingleton<IPostUpgradeProcessHandler, NoCustomProcessesHandler>()
            .AddSingleton<IPreUpgradeProcessHandler, NoCustomProcessesHandler>();
        return services;
    }
    public static IServiceCollection RegisterNoPostUpgradeCommands(this IServiceCollection services)
    {
        services.AddSingleton<IPostBuildCommandStrategy, NoPostBuildCommandStrategy>();
        return services;
    }
}
namespace UpdateManager.DotNetUpgradeLibrary.Extensions;
public static class ServiceExtensions
{
    public static IServiceCollection RegisterDotNetUpgradeServices(this IServiceCollection services, bool useFileBased = true, bool alsoBuilder = true, bool alsonugetPacker = true, Action<IServiceCollection>? additionalServices = null)
    {
        services.AddSingleton<IFeedPathResolver, FeedPathResolver>()
            .AddSingleton<ITestFileManager, TestFileManager>()
            //.AddSingleton<ITemplateNetUpdater, TemplateNetUpdater>()
            .AddSingleton<IBranchValidationService, BranchValidationService>()
            .AddSingleton<ILibraryDotNetUpgradeCommitter, LibraryDotNetUpgradeCommitter>()
            .AddSingleton<ILibraryNetUpdateModelGenerator, LibraryNetUpdateModelGenerator>()
            .AddSingleton<IPackageFeedManager, PackageFeedManager>()
            .AddSingleton<IDotNetVersionUpdater, DotNetVersionUpdater>()
            .AddSingleton<IUpgradePhaseHandler, UpgradePhaseManager>()
            .AddSingleton<DotNetUpgradeCoordinator>();
        if (alsoBuilder)
        {
            services.AddSingleton<ILibraryDotNetUpgraderBuild, LibraryDotNetUpgraderBuild>();
        }
        if (alsonugetPacker)
        {
            services.AddSingleton<INugetPacker, NugetPacker>();
        }
        if (useFileBased)
        {
            services.AddSingleton<IDotNetUpgradeConfigReader, DotNetUpgradeConfigReader>()
                .AddSingleton<INetVersionUpdateContext, FileNetVersionUpdateContext>()
                .AddSingleton<IPackagesContext, FilePackagesContext>();
        }
        additionalServices?.Invoke(services);
        return services;
    }
    public static IServiceCollection RegisterDefaultUpgradeFactory(this IServiceCollection services)
    {
        services.AddSingleton<IUpgradePhaseFactory, EmptyUpgradePhaseFactory>();
        return services;
    }
    public static IServiceCollection RegisterNoPostUpgradeCommands(this IServiceCollection services)
    {
        services.AddSingleton<IPostBuildCommandStrategy, NoPostBuildCommandStrategy>();
        return services;
    }
    public static IServiceCollection RegisterPostUpgradeOnlyServices<T>(this IServiceCollection services, Action<IServiceCollection> actions, bool useFileBased = true)
        where T: class, IUpgradePhaseFactory
    {
        services.AddSingleton<PostUpgradeCoordinator>()
            .AddSingleton<IUpgradePhaseFactory, T>();
        if (useFileBased)
        {
            services.AddSingleton<IPackagesContext, FilePackagesContext>();
        }
        actions.Invoke(services);
        return services;
    }
}
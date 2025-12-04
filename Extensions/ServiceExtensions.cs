namespace UpdateManager.DotNetUpgradeLibrary.Extensions;
public static class ServiceExtensions
{
    extension (IServiceCollection services)
    {
        public IServiceCollection RegisterDotNetUpgradeServices(bool useFileBased = true, bool alsoBuilder = true, bool alsonugetPacker = true, Action<IServiceCollection>? additionalServices = null)
        {
            services.AddSingleton<IFeedPathResolver, FeedPathResolver>()
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
                services.AddSingleton<INetVersionUpdateContext, FileNetVersionUpdateContext>()
                    .AddSingleton<IPackagesContext, FilePackagesContext>();
            }
            additionalServices?.Invoke(services);
            return services;
        }
        public IServiceCollection RegisterDefaultUpgradeFactory()
        {
            services.AddSingleton<IUpgradePhaseFactory, EmptyUpgradePhaseFactory>();
            return services;
        }
        public IServiceCollection RegisterNoPostUpgradeCommands()
        {
            services.AddSingleton<IPostBuildCommandStrategy, NoPostBuildCommandStrategy>();
            return services;
        }
        public IServiceCollection RegisterPostUpgradeOnlyServices<T>(Action<IServiceCollection> actions, bool useFileBased = true)
            where T : class, IUpgradePhaseFactory
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
    
}
using Autofac;
using LightSync.Core.Areas;
using LightSync.Core.Captures;
using LightSync.Core.Lights;
using LightSync.Core.Paths;
using LightSync.Core.Plugins;
using LightSync.Core.Progress;
using LightSync.UI.Dialogs;
using LightSync.UI.Navigation;
using LightSync.UI.Plugins;
using LightSync.UI.ViewModels;
using LightSync.UI.ViewModels.Dialogs;
using LightSync.UI.ViewModels.Pages;

namespace LightSync.UI.Containers
{
    internal static class ContainerConfiguration
    {
        public static void Configure(ContainerBuilder builder)
        {
            ConfigureSingleInstances(builder);
            ConfigureInstances(builder);
            ConfigureViewModels(builder);
            ConfigureDialogs(builder);
        }

        private static void ConfigureSingleInstances(ContainerBuilder builder)
        {
            builder.RegisterType<Navigator>().As<INavigator>().SingleInstance();
            builder.RegisterType<DialogPresenter>().As<IDialogService>().SingleInstance();
            builder.RegisterType<PluginManager>().As<IPluginManager>().SingleInstance();
            builder.RegisterType<CommonPaths>().As<ICommonPaths>().SingleInstance();
            builder.RegisterType<PluginLoader>().As<IPluginLoader>().SingleInstance();
            builder.RegisterType<PluginManifestLoader>().As<IPluginManifestLoader>().SingleInstance();
            builder.RegisterType<ProgressInfoFactory>().As<IProgressInfoFactory>().SingleInstance();
            builder.RegisterType<ProgressService>().As<IProgressService>().SingleInstance();
            builder.RegisterType<AreaDefinitionFactory>().As<IAreaDefinitionFactory>().SingleInstance();
            builder.RegisterType<LightPlacementInfoFactory>().As<ILightPlacementInfoFactory>().SingleInstance();
            builder.RegisterType<AreaManager>().As<IAreaManager>().SingleInstance();
            builder.RegisterType<CaptureManager>().As<ICaptureManager>().SingleInstance();
        }

        private static void ConfigureInstances(ContainerBuilder builder)
        {
        }

        private static void ConfigureViewModels(ContainerBuilder builder)
        {
            builder.RegisterType<MainWindowViewModel>().As<IMainWindowViewModel>();

            builder.RegisterType<AreasPageViewModel>().As<IAreasPageViewModel>();
            builder.RegisterType<EditAreaPageViewModel>().As<IEditAreaPageViewModel>();
            builder.RegisterType<LightsPageViewModel>().As<ILightsPageViewModel>();
            builder.RegisterType<SettingsPageViewModel>().As<ISettingsPageViewModel>();
            builder.RegisterType<SourcesPageViewModel>().As<ISourcesPageViewModel>();
        }

        private static void ConfigureDialogs(ContainerBuilder builder)
        {
            builder.RegisterType<SimpleDialogViewModel>().As<ISimpleDialogViewModel>();
        }
    }
}

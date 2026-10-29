using Autofac;
using LightSync.UI.ViewModels;
using LightSync.UI.ViewModels.Pages;
using System;

namespace LightSync.UI.Containers
{
    internal static class DesignDataConfiguration
    {
        private static readonly Lazy<IContainer> Container = new(() =>
        {
            var builder = new ContainerBuilder();

            ContainerConfiguration.Configure(builder);

            return builder.Build();
        });

        public static IMainWindowViewModel MainWindow => Container.Value.Resolve<IMainWindowViewModel>();
        public static IAreasPageViewModel AreasPage => Container.Value.Resolve<IAreasPageViewModel>();
        public static IEditAreaPageViewModel EditAreaPage => Container.Value.Resolve<IEditAreaPageViewModel>();
        public static ILightsPageViewModel LightsPage => Container.Value.Resolve<ILightsPageViewModel>();
        public static ISettingsPageViewModel SettingsPage => Container.Value.Resolve<ISettingsPageViewModel>();
        public static ISourcesPageViewModel SourcesPage => Container.Value.Resolve<ISourcesPageViewModel>();
    }
}

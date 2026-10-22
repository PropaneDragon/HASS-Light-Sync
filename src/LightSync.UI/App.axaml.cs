using Autofac;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LightSync.Core.Paths;
using LightSync.Core.Plugins;
using LightSync.UI.Dialogs;
using LightSync.UI.Navigation;
using LightSync.UI.Plugins;
using LightSync.UI.Progress;
using LightSync.UI.ViewModels;
using LightSync.UI.ViewModels.Dialogs;
using LightSync.UI.ViewModels.Pages;
using LightSync.UI.Views;
using System;

namespace LightSync.UI
{
    public partial class App : Application
    {
        private IContainer? _container;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (!Design.IsDesignMode)
            {
                var builder = new ContainerBuilder();

                ConfigureContainer(builder);

                _container = builder.Build();

                if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    var viewModel = _container.Resolve<IMainWindowViewModel>();
                    var mainWindow = new MainWindow
                    {
                        DataContext = viewModel
                    };

                    mainWindow.Opened += async (_, _) =>
                    {
                        try { await viewModel.OnLoadedAsync(); }
                        catch { } //Todo
                    };

                    desktop.MainWindow = mainWindow;
                    desktop.Exit += (_, _) => _container.Dispose();
                }
            }

            base.OnFrameworkInitializationCompleted();
        }

        private static void ConfigureContainer(ContainerBuilder builder)
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
        }

        private static void ConfigureInstances(ContainerBuilder builder)
        {
            builder.RegisterType<ProgressInfo>().As<IProgressInfo>();
        }

        private static void ConfigureViewModels(ContainerBuilder builder)
        {
            builder.RegisterType<MainWindowViewModel>().As<IMainWindowViewModel>();

            builder.RegisterType<AreasPageViewModel>().As<IAreasPageViewModel>();
            builder.RegisterType<LightsPageViewModel>().As<ILightsPageViewModel>();
            builder.RegisterType<SettingsPageViewModel>().As<ISettingsPageViewModel>();
            builder.RegisterType<SourcesPageViewModel>().As<ISourcesPageViewModel>();
        }

        private static void ConfigureDialogs(ContainerBuilder builder)
        {
        }
    }
}
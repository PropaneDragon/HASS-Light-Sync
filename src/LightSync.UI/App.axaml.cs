using Autofac;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LightSync.Core.Paths;
using LightSync.Core.Plugins;
using LightSync.UI.Navigation;
using LightSync.UI.Plugins;
using LightSync.UI.ViewModels;
using LightSync.UI.ViewModels.Pages;
using LightSync.UI.Views;

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
            builder.RegisterType<Navigator>().As<INavigator>().SingleInstance();
            builder.RegisterType<PluginManager>().As<IPluginManager>().SingleInstance();
            builder.RegisterType<CommonPaths>().As<ICommonPaths>().SingleInstance();
            builder.RegisterType<PluginLoader>().As<IPluginLoader>().SingleInstance();
            builder.RegisterType<PluginManifestLoader>().As<IPluginManifestLoader>().SingleInstance();

            builder.RegisterType<MainWindowViewModel>().As<IMainWindowViewModel>();

            builder.RegisterType<HomePageViewModel>().As<IHomePageViewModel>();
            builder.RegisterType<AreaConfigurationViewModel>().As<IAreaConfigurationViewModel>();
        }
    }
}
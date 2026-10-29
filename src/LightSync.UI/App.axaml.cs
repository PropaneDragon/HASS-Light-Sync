using Autofac;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LightSync.UI.Containers;
using LightSync.UI.ViewModels;
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

                ContainerConfiguration.Configure(builder);

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
    }
}
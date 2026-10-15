using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LightSync.UI.ViewModels;
using LightSync.UI.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace LightSync.UI
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            HostApplicationBuilder builder = Host.CreateEmptyApplicationBuilder(new HostApplicationBuilderSettings
            {
                DisableDefaults = true,
                Args = Environment.GetCommandLineArgs(),
                ApplicationName = base.Name,
                ContentRootPath = AppContext.BaseDirectory,
                Configuration = new ConfigurationManager()
            });

            if (!Design.IsDesignMode)
            {
                ConfigureServices(builder.Services, builder.Configuration, builder.Environment);
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void ConfigureServices(IServiceCollection services, ConfigurationManager configuration, IHostEnvironment environment)
        {

        }
    }
}
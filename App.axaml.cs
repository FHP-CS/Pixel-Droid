// App.axaml.cs
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using PixelWallE.ViewModels; // Your namespaces
using PixelWallE.Views;

namespace PixelWallE;

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
                // Create and assign the ViewModel instance here
                DataContext = new MainWindowViewModel(),
            };
        }
        // else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        // {
        //    singleViewPlatform.MainView = new MainView // Adjust for single view platforms if needed
        //    {
        //        DataContext = new MainViewModel(),
        //    };
        // }

        base.OnFrameworkInitializationCompleted();
    }
}
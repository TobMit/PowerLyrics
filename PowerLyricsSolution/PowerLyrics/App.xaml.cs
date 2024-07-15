using System;
using System.Windows;
using PowerLyrics.Windows;

namespace PowerLyrics;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        var currentDomain = AppDomain.CurrentDomain;
        currentDomain.UnhandledException += MyHandler;
    }

    /// <summary>
    /// Handle výnimku
    /// </summary>
    private static void MyHandler(object sender, UnhandledExceptionEventArgs args)
    {
        var e = (Exception)args.ExceptionObject;
        MessageBox.Show("We are sorry for crashing. This app is still in progress!\n\n" + e.Message, "App crash",
            MessageBoxButton.OK, MessageBoxImage.Error);
    }

    /// <summary>
    /// Otvorenie aplikácie po otvorení súboru
    /// </summary>
    private void App_OnStartup(object sender, StartupEventArgs e)
    {
        var mainWindow = new MainWindow();
        mainWindow.Show();

        if (e.Args.Length > 0) mainWindow.myDataContext.openSongOnStartup(e.Args[0]);
    }
}
using Avalonia;

namespace StockMaster;

/// <summary>
/// Punto de entrada de la aplicación. Solo configura Avalonia y la arranca.
/// </summary>
internal static class Program
{
    [STAThread]
    public static void Main(string[] args) =>
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
                  .UsePlatformDetect();
}

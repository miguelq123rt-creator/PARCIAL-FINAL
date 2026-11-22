using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using StockMaster.Data;
using StockMaster.Views;

namespace StockMaster;

/// <summary>
/// Clase de la aplicación. Al iniciar crea la base de datos (si no existe)
/// y abre la ventana principal.
/// </summary>
public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // 1. Preparar la base de datos antes de mostrar nada
        InicializadorBD.Inicializar();

        // 2. Abrir la ventana principal
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}

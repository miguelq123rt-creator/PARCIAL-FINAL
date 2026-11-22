using Avalonia.Controls;
using Avalonia.Interactivity;

namespace StockMaster.Views;

/// <summary>
/// Ventana pequeña que pregunta "¿Seguro que quieres eliminar?".
/// Devuelve true si el usuario confirma y false si cancela o cierra la ventana.
/// </summary>
public partial class ConfirmarWindow : Window
{
    // Avalonia necesita un constructor sin parámetros
    public ConfirmarWindow()
    {
        InitializeComponent();
    }

    public ConfirmarWindow(string mensaje) : this()
    {
        TxtMensaje.Text = mensaje;
    }

    private void Si_Click(object? sender, RoutedEventArgs e) => Close(true);

    private void No_Click(object? sender, RoutedEventArgs e) => Close(false);
}

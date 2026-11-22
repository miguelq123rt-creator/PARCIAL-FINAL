using System.Globalization;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using StockMaster.Data;
using StockMaster.Models;
using StockMaster.Services;

namespace StockMaster.Views;

/// <summary>
/// Ventana principal (capa de INTERFAZ).
/// Solo se encarga de: leer lo que escribió el usuario, llamar al servicio
/// y mostrar el resultado. No contiene SQL ni reglas de negocio.
/// Los métodos "..._Click" son los EVENTOS declarados en el archivo .axaml.
/// </summary>
public partial class MainWindow : Window
{
    private readonly ProductoServicio _servicio = new(new ProductoDAO());

    // Si es null estamos creando un producto nuevo;
    // si tiene valor, estamos editando ese producto.
    private Producto? _productoEnEdicion;

    public MainWindow()
    {
        InitializeComponent();

        TxtRuta.Text = "Los datos se guardan en: " + ConexionBD.RutaBaseDatos;
        CargarProductos();
        ActualizarModoFormulario();
    }

    // =====================  EVENTOS  =====================

    // Botón "Guardar producto" / "Actualizar producto"
    private void Guardar_Click(object? sender, RoutedEventArgs e)
    {
        // 1. Leer lo que escribió el usuario
        string nombre = TxtNombre.Text ?? "";
        string categoria = TxtCategoria.Text ?? "";
        string precio = TxtPrecio.Text ?? "";
        string stock = TxtStock.Text ?? "";

        // 2. Pedirle al servicio que valide y guarde (crear o actualizar)
        ResultadoOperacion resultado = _productoEnEdicion == null
            ? _servicio.Registrar(nombre, categoria, precio, stock)
            : _servicio.Actualizar(_productoEnEdicion.Id, nombre, categoria, precio, stock);

        // 3. Mostrar el resultado; si salió bien, limpiar y refrescar la tabla
        if (resultado.Exito)
        {
            LimpiarFormulario();
            CargarProductos();
        }
        MostrarMensaje(resultado.Mensaje, resultado.Exito);
    }

    // Botón "Nuevo / Limpiar"
    private void Limpiar_Click(object? sender, RoutedEventArgs e)
    {
        LimpiarFormulario();
        OcultarMensaje();
    }

    // Botón "Eliminar" (pide confirmación antes de borrar)
    private async void Eliminar_Click(object? sender, RoutedEventArgs e)
    {
        if (_productoEnEdicion == null)
        {
            MostrarMensaje("Primero selecciona un producto de la tabla.", false);
            return;
        }

        var dialogo = new ConfirmarWindow(
            $"¿Seguro que quieres eliminar \"{_productoEnEdicion.Nombre}\"? Esta acción no se puede deshacer.");
        bool confirmado = await dialogo.ShowDialog<bool>(this);
        if (!confirmado)
            return;

        ResultadoOperacion resultado = _servicio.Eliminar(_productoEnEdicion.Id);
        if (resultado.Exito)
        {
            LimpiarFormulario();
            CargarProductos();
        }
        MostrarMensaje(resultado.Mensaje, resultado.Exito);
    }

    // Se ejecuta cada vez que el usuario escribe en el buscador
    private void Buscar_TextChanged(object? sender, TextChangedEventArgs e)
    {
        CargarProductos();
    }

    // Se ejecuta al hacer clic en una fila de la tabla: pasa sus datos al formulario
    private void Tabla_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (TablaProductos.SelectedItem is not Producto producto)
            return;

        _productoEnEdicion = producto;
        TxtNombre.Text = producto.Nombre;
        TxtCategoria.Text = producto.Categoria;
        TxtPrecio.Text = producto.Precio.ToString("0.00", CultureInfo.InvariantCulture);
        TxtStock.Text = producto.Stock.ToString();

        OcultarMensaje();
        ActualizarModoFormulario();
    }

    // =====================  MÉTODOS DE APOYO  =====================

    // Lee los productos de la base de datos y actualiza tabla + tarjetas
    private void CargarProductos()
    {
        try
        {
            List<Producto> filtrados = _servicio.Consultar(TxtBuscar.Text);
            List<Producto> todos = _servicio.Consultar(null);

            TablaProductos.ItemsSource = filtrados;
            TxtSinDatos.IsVisible = filtrados.Count == 0;

            ResumenInventario resumen = _servicio.CalcularResumen(todos);
            TxtTotalProductos.Text = resumen.TotalProductos.ToString();
            TxtUnidades.Text = resumen.TotalUnidades.ToString();
            TxtValor.Text = $"$ {resumen.ValorTotal:N2}";
            TxtAlertas.Text = resumen.ProductosEnAlerta.ToString();
        }
        catch (Exception ex)
        {
            MostrarMensaje("No se pudieron cargar los productos: " + ex.Message, false);
        }
    }

    private void LimpiarFormulario()
    {
        _productoEnEdicion = null;
        TablaProductos.SelectedItem = null;
        TxtNombre.Text = "";
        TxtCategoria.Text = "";
        TxtPrecio.Text = "";
        TxtStock.Text = "";
        ActualizarModoFormulario();
        TxtNombre.Focus();
    }

    // Cambia textos y botones según estemos creando o editando
    private void ActualizarModoFormulario()
    {
        bool editando = _productoEnEdicion != null;
        TxtTituloForm.Text = editando ? "Editar producto" : "Nuevo producto";
        BtnGuardar.Content = editando ? "Actualizar producto" : "Guardar producto";
        BtnEliminar.IsEnabled = editando;
    }

    private void MostrarMensaje(string texto, bool exito)
    {
        TxtMensaje.Text = texto;
        TxtMensaje.Foreground = Brush.Parse(exito ? "#166534" : "#991B1B");
        MensajeBorder.Background = Brush.Parse(exito ? "#DCFCE7" : "#FEE2E2");
        MensajeBorder.IsVisible = true;
    }

    private void OcultarMensaje()
    {
        MensajeBorder.IsVisible = false;
    }
}

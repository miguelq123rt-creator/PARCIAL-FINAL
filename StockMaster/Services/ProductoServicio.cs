using System.Globalization;
using Microsoft.Data.Sqlite;
using StockMaster.Data;
using StockMaster.Models;

namespace StockMaster.Services;

/// <summary>
/// Capa de LÓGICA del programa (reglas de negocio). Está entre la interfaz y el DAO:
///   Ventana  ->  ProductoServicio  ->  ProductoDAO  ->  SQLite
/// Qué hace:
///   1. Convierte el texto de las cajas (precio, stock) a números.
///   2. Valida los datos (las reglas básicas viven en la clase Producto).
///   3. Revisa reglas extra, como no repetir nombres.
///   4. Llama al DAO y devuelve un ResultadoOperacion con el mensaje para el usuario.
/// La ventana NUNCA toca la base de datos directamente.
/// </summary>
public class ProductoServicio
{
    private readonly ProductoDAO _dao;

    public ProductoServicio(ProductoDAO dao)
    {
        _dao = dao;
    }

    // ---------- Registrar (Create) ----------
    public ResultadoOperacion Registrar(string nombre, string categoria, string precioTexto, string stockTexto)
    {
        var (producto, error) = CrearProducto(0, nombre, categoria, precioTexto, stockTexto);
        if (producto == null)
            return ResultadoOperacion.Error(error!);

        try
        {
            if (_dao.ExisteNombre(producto.Nombre))
                return ResultadoOperacion.Error("Ya existe un producto con ese nombre.");

            _dao.Insertar(producto);
            return ResultadoOperacion.Ok($"Producto \"{producto.Nombre}\" registrado correctamente.");
        }
        catch (SqliteException ex)
        {
            return ResultadoOperacion.Error("Error de base de datos: " + ex.Message);
        }
    }

    // ---------- Consultar (Read) ----------
    public List<Producto> Consultar(string? filtro)
    {
        return string.IsNullOrWhiteSpace(filtro)
            ? _dao.ObtenerTodos()
            : _dao.Buscar(filtro.Trim());
    }

    // ---------- Actualizar (Update) ----------
    public ResultadoOperacion Actualizar(int id, string nombre, string categoria, string precioTexto, string stockTexto)
    {
        var (producto, error) = CrearProducto(id, nombre, categoria, precioTexto, stockTexto);
        if (producto == null)
            return ResultadoOperacion.Error(error!);

        try
        {
            if (_dao.ExisteNombre(producto.Nombre, id))
                return ResultadoOperacion.Error("Ya existe otro producto con ese nombre.");

            bool modificado = _dao.Actualizar(producto);
            return modificado
                ? ResultadoOperacion.Ok($"Producto \"{producto.Nombre}\" actualizado correctamente.")
                : ResultadoOperacion.Error("No se encontró el producto (quizá ya fue eliminado).");
        }
        catch (SqliteException ex)
        {
            return ResultadoOperacion.Error("Error de base de datos: " + ex.Message);
        }
    }

    // ---------- Eliminar (Delete) ----------
    public ResultadoOperacion Eliminar(int id)
    {
        try
        {
            bool eliminado = _dao.Eliminar(id);
            return eliminado
                ? ResultadoOperacion.Ok("Producto eliminado correctamente.")
                : ResultadoOperacion.Error("No se encontró el producto (quizá ya fue eliminado).");
        }
        catch (SqliteException ex)
        {
            return ResultadoOperacion.Error("Error de base de datos: " + ex.Message);
        }
    }

    // ---------- Resumen para las tarjetas ----------
    public ResumenInventario CalcularResumen(List<Producto> productos)
    {
        return new ResumenInventario(
            TotalProductos: productos.Count,
            TotalUnidades: productos.Sum(p => p.Stock),
            ValorTotal: productos.Sum(p => p.CalcularValorTotal()),
            ProductosEnAlerta: productos.Count(p => p.EstaAgotado || p.TieneStockBajo));
    }

    // ---------- Validación y conversión ----------
    /// <summary>
    /// Convierte los textos del formulario en un objeto Producto.
    /// Si algo está mal devuelve (null, mensaje de error).
    /// </summary>
    private static (Producto? producto, string? error) CrearProducto(
        int id, string nombre, string categoria, string precioTexto, string stockTexto)
    {
        // Aceptamos "12.50" y "12,50"
        string precioLimpio = precioTexto.Trim().Replace(',', '.');
        if (!decimal.TryParse(precioLimpio, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal precio))
            return (null, "El precio debe ser un número válido (ejemplo: 12.50).");

        if (!int.TryParse(stockTexto.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int stock))
            return (null, "El stock debe ser un número entero (ejemplo: 20).");

        try
        {
            // El constructor de Producto aplica las reglas de sus propiedades
            return (new Producto(id, nombre, categoria, precio, stock), null);
        }
        catch (ArgumentException ex)
        {
            return (null, ex.Message);
        }
    }
}

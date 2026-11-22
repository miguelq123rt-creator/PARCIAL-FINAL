using Microsoft.Data.Sqlite;
using StockMaster.Models;

namespace StockMaster.Data;

/// <summary>
/// DAO (Data Access Object): ÚNICA clase que habla con la base de datos.
/// Aquí están las 4 operaciones CRUD con ADO.NET:
///   Create -> Insertar     Read -> ObtenerTodos / Buscar
///   Update -> Actualizar   Delete -> Eliminar
/// Todas las consultas usan PARÁMETROS ($nombre, $id...) en lugar de pegar
/// texto del usuario dentro del SQL; así se evita la inyección SQL.
/// Cada método abre su conexión con "using", que la cierra automáticamente.
/// </summary>
public class ProductoDAO
{
    // ---------- CREATE ----------
    public void Insertar(Producto producto)
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        using var comando = conexion.CreateCommand();
        comando.CommandText = @"
            INSERT INTO Productos (Nombre, Categoria, Precio, Stock)
            VALUES ($nombre, $categoria, $precio, $stock);";
        comando.Parameters.AddWithValue("$nombre", producto.Nombre);
        comando.Parameters.AddWithValue("$categoria", producto.Categoria);
        comando.Parameters.AddWithValue("$precio", (double)producto.Precio);
        comando.Parameters.AddWithValue("$stock", producto.Stock);
        comando.ExecuteNonQuery();
    }

    // ---------- READ ----------
    public List<Producto> ObtenerTodos()
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        using var comando = conexion.CreateCommand();
        comando.CommandText = @"
            SELECT Id, Nombre, Categoria, Precio, Stock
            FROM Productos
            ORDER BY Nombre COLLATE NOCASE;";

        return LeerProductos(comando);
    }

    public List<Producto> Buscar(string texto)
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        using var comando = conexion.CreateCommand();
        comando.CommandText = @"
            SELECT Id, Nombre, Categoria, Precio, Stock
            FROM Productos
            WHERE Nombre LIKE $texto OR Categoria LIKE $texto
            ORDER BY Nombre COLLATE NOCASE;";
        comando.Parameters.AddWithValue("$texto", "%" + texto + "%");

        return LeerProductos(comando);
    }

    // ---------- UPDATE ----------
    /// <returns>true si se modificó una fila; false si el Id no existe.</returns>
    public bool Actualizar(Producto producto)
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        using var comando = conexion.CreateCommand();
        comando.CommandText = @"
            UPDATE Productos
            SET Nombre = $nombre, Categoria = $categoria, Precio = $precio, Stock = $stock
            WHERE Id = $id;";
        comando.Parameters.AddWithValue("$nombre", producto.Nombre);
        comando.Parameters.AddWithValue("$categoria", producto.Categoria);
        comando.Parameters.AddWithValue("$precio", (double)producto.Precio);
        comando.Parameters.AddWithValue("$stock", producto.Stock);
        comando.Parameters.AddWithValue("$id", producto.Id);

        return comando.ExecuteNonQuery() > 0; // filas afectadas
    }

    // ---------- DELETE ----------
    /// <returns>true si se eliminó una fila; false si el Id no existe.</returns>
    public bool Eliminar(int id)
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        using var comando = conexion.CreateCommand();
        comando.CommandText = "DELETE FROM Productos WHERE Id = $id;";
        comando.Parameters.AddWithValue("$id", id);

        return comando.ExecuteNonQuery() > 0;
    }

    // ---------- Apoyo ----------
    /// <summary>Indica si ya hay otro producto con ese nombre (sin distinguir mayúsculas).</summary>
    /// <param name="idExcluir">Id del producto que se está editando (para no compararlo consigo mismo).</param>
    public bool ExisteNombre(string nombre, int idExcluir = 0)
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        using var comando = conexion.CreateCommand();
        comando.CommandText = @"
            SELECT COUNT(*) FROM Productos
            WHERE Nombre = $nombre COLLATE NOCASE AND Id <> $id;";
        comando.Parameters.AddWithValue("$nombre", nombre);
        comando.Parameters.AddWithValue("$id", idExcluir);

        return Convert.ToInt64(comando.ExecuteScalar()) > 0;
    }

    // Convierte cada fila del resultado (SqliteDataReader) en un objeto Producto
    private static List<Producto> LeerProductos(SqliteCommand comando)
    {
        var lista = new List<Producto>();

        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            lista.Add(new Producto(
                lector.GetInt32(0),
                lector.GetString(1),
                lector.GetString(2),
                (decimal)lector.GetDouble(3),
                lector.GetInt32(4)));
        }
        return lista;
    }
}

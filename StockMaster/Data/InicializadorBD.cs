namespace StockMaster.Data;

/// <summary>
/// Crea la tabla Productos la primera vez que se ejecuta el programa.
/// "IF NOT EXISTS" hace que sea seguro llamarlo en cada arranque:
/// si la tabla ya existe, no se toca y los datos se conservan.
/// (El mismo SQL está en Database/schema.sql como referencia.)
/// </summary>
public static class InicializadorBD
{
    public static void Inicializar()
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        using var comando = conexion.CreateCommand();
        comando.CommandText = @"
            CREATE TABLE IF NOT EXISTS Productos (
                Id        INTEGER PRIMARY KEY AUTOINCREMENT,
                Nombre    TEXT    NOT NULL,
                Categoria TEXT    NOT NULL,
                Precio    REAL    NOT NULL CHECK (Precio > 0),
                Stock     INTEGER NOT NULL CHECK (Stock >= 0)
            );";
        comando.ExecuteNonQuery();
    }
}

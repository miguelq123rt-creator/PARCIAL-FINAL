using Microsoft.Data.Sqlite;

namespace StockMaster.Data;

/// <summary>
/// Responsabilidad única: saber DÓNDE está el archivo de la base de datos
/// y crear conexiones ADO.NET hacia él.
/// El archivo se guarda en la carpeta de datos del usuario, así que no se
/// borra al limpiar/recompilar el proyecto y los datos persisten.
/// </summary>
public static class ConexionBD
{
    public static string RutaBaseDatos { get; } = ObtenerRuta();

    private static string ObtenerRuta()
    {
        string carpeta = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "StockMaster");

        Directory.CreateDirectory(carpeta); // no falla si ya existe
        return Path.Combine(carpeta, "stockmaster.db");
    }

    /// <summary>Crea una conexión nueva (todavía cerrada). Quien la use debe abrirla y cerrarla.</summary>
    public static SqliteConnection CrearConexion()
    {
        return new SqliteConnection($"Data Source={RutaBaseDatos}");
    }
}

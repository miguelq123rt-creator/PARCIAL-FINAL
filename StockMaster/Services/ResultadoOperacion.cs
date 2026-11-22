namespace StockMaster.Services;

/// <summary>
/// Respuesta que el servicio le devuelve a la interfaz:
/// ¿salió bien? y ¿qué mensaje se le muestra al usuario?
/// </summary>
public class ResultadoOperacion
{
    public bool Exito { get; }
    public string Mensaje { get; }

    private ResultadoOperacion(bool exito, string mensaje)
    {
        Exito = exito;
        Mensaje = mensaje;
    }

    public static ResultadoOperacion Ok(string mensaje) => new(true, mensaje);
    public static ResultadoOperacion Error(string mensaje) => new(false, mensaje);
}

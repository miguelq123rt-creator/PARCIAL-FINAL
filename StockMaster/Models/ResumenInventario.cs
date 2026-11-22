namespace StockMaster.Models;

/// <summary>
/// Datos de resumen que se muestran en las tarjetas de la parte superior.
/// Es un "record": una clase pequeña e inmutable pensada para llevar datos.
/// </summary>
public record ResumenInventario(
    int TotalProductos,
    int TotalUnidades,
    decimal ValorTotal,
    int ProductosEnAlerta);

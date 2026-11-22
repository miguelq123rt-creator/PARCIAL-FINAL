namespace StockMaster.Models;

/// <summary>
/// Representa un producto de la tienda.
/// POO: es la clase principal del dominio. Usa ENCAPSULAMIENTO: los campos son
/// privados y las propiedades validan los datos antes de guardarlos, así
/// es imposible tener un producto con precio negativo o sin nombre.
/// </summary>
public class Producto
{
    /// <summary>Si el stock es menor o igual a este número se considera "bajo".</summary>
    public const int UmbralStockBajo = 5;

    // Campos privados (encapsulados)
    private string _nombre = "";
    private string _categoria = "";
    private decimal _precio;
    private int _stock;

    // Constructor para productos NUEVOS (todavía sin Id; lo asigna SQLite)
    public Producto(string nombre, string categoria, decimal precio, int stock)
        : this(0, nombre, categoria, precio, stock)
    {
    }

    // Constructor para productos que YA existen en la base de datos
    public Producto(int id, string nombre, string categoria, decimal precio, int stock)
    {
        Id = id;
        Nombre = nombre;       // pasa por las validaciones de la propiedad
        Categoria = categoria;
        Precio = precio;
        Stock = stock;
    }

    public int Id { get; set; }

    public string Nombre
    {
        get => _nombre;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El nombre del producto es obligatorio.");
            if (value.Trim().Length > 80)
                throw new ArgumentException("El nombre no puede superar los 80 caracteres.");
            _nombre = value.Trim();
        }
    }

    public string Categoria
    {
        get => _categoria;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("La categoría es obligatoria.");
            if (value.Trim().Length > 40)
                throw new ArgumentException("La categoría no puede superar los 40 caracteres.");
            _categoria = value.Trim();
        }
    }

    public decimal Precio
    {
        get => _precio;
        set
        {
            if (value <= 0)
                throw new ArgumentException("El precio debe ser mayor que 0.");
            if (value > 1_000_000_000m)
                throw new ArgumentException("El precio es demasiado alto.");
            _precio = Math.Round(value, 2);
        }
    }

    public int Stock
    {
        get => _stock;
        set
        {
            if (value < 0)
                throw new ArgumentException("El stock no puede ser negativo.");
            if (value > 1_000_000)
                throw new ArgumentException("El stock es demasiado alto.");
            _stock = value;
        }
    }

    // ---- Propiedades calculadas (la tabla de la interfaz las usa) ----
    public bool EstaAgotado => Stock == 0;
    public bool TieneStockBajo => Stock > 0 && Stock <= UmbralStockBajo;
    public bool TieneStockNormal => Stock > UmbralStockBajo;

    /// <summary>Valor total en dinero de las unidades de este producto.</summary>
    public decimal CalcularValorTotal() => Precio * Stock;

    public override string ToString() => $"{Nombre} ({Categoria}) - {Stock} u.";
}

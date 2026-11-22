# Guía para explicar StockMaster

## La idea en una frase
"Es una aplicación de escritorio hecha con Avalonia que guarda productos en SQLite usando ADO.NET, organizada en tres capas: interfaz, lógica y datos."

## Las tres capas
| Capa | Carpeta | Pregunta que responde |
|---|---|---|
| Interfaz | `Views/` | ¿Qué ve y toca el usuario? |
| Lógica | `Services/` | ¿Los datos son válidos? ¿Qué reglas aplican? |
| Datos | `Data/` | ¿Cómo se guarda y se lee de SQLite? |

Más `Models/` con `Producto`, que es el "objeto" que viaja entre capas.

## Recorrido: el usuario pulsa "Guardar"
1. `MainWindow.Guardar_Click` lee las cajas de texto.
2. Llama a `ProductoServicio.Registrar`.
3. El servicio convierte precio y stock a números y crea un `Producto`; si algo es inválido, las propiedades de `Producto` lanzan un `ArgumentException` con el mensaje.
4. El servicio revisa que el nombre no esté repetido (`ExisteNombre`).
5. `ProductoDAO.Insertar` abre la conexión, ejecuta el `INSERT` con parámetros y la cierra.
6. El servicio devuelve un `ResultadoOperacion`.
7. La ventana muestra el mensaje (verde/rojo) y recarga la tabla.

## Preguntas probables
- **¿Por qué ADO.NET y no Entity Framework?** El parcial lo exige; además, escribir el SQL a mano muestra cómo funciona la conexión.
- **¿Qué son los parámetros (`$nombre`)?** Evitan la inyección SQL: el valor del usuario nunca se concatena dentro de la consulta.
- **¿Para qué sirve `using`?** Cierra y libera la conexión automáticamente, incluso si hay un error.
- **¿Dónde está el encapsulamiento?** En `Producto`: campos privados y propiedades que validan.
- **¿Por qué el DAO está separado?** Para que solo una clase conozca el SQL. Si cambiara la base de datos, solo se tocaría el DAO.
- **¿Por qué no se pierden los datos al cerrar?** SQLite guarda en un archivo `.db` en disco, no en memoria.
- **¿Cómo se crea la base de datos?** `InicializadorBD` ejecuta `CREATE TABLE IF NOT EXISTS` al arrancar.
- **¿Qué es un evento?** Un método que se ejecuta cuando el usuario hace algo; por ejemplo `Click="Guardar_Click"` en el `.axaml` enlaza el botón con ese método de C#.
- **¿Qué es el `.axaml`?** El archivo XML que describe cómo se ve la ventana. El `.axaml.cs` asociado contiene el código C# de esa ventana.

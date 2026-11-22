-- StockMaster: esquema de la base de datos SQLite
-- Este script es de REFERENCIA. La aplicación ejecuta este mismo SQL
-- automáticamente al iniciar (ver Data/InicializadorBD.cs), por lo que
-- NO es necesario ejecutarlo a mano.

CREATE TABLE IF NOT EXISTS Productos (
    Id        INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre    TEXT    NOT NULL,
    Categoria TEXT    NOT NULL,
    Precio    REAL    NOT NULL CHECK (Precio > 0),
    Stock     INTEGER NOT NULL CHECK (Stock >= 0)
);

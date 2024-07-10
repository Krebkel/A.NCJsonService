using Npgsql;

namespace Web;

/// <summary>
/// Первичная инициализация базы данных
/// </summary>
public static class DatabaseInitializer
{
    /// <summary>
    /// Запросы на создание таблиц
    /// </summary>
    public static void CreateTables(NpgsqlConnection connection)
    {
        var createWareTable = @"
                CREATE TABLE IF NOT EXISTS Wares (
                    Id SERIAL PRIMARY KEY,
                    Name TEXT UNIQUE NOT NULL,
                    Value DECIMAL NOT NULL,
                    Property TEXT
                );";

        var createOrderTable = @"
                CREATE TABLE IF NOT EXISTS Orders (
                    Id SERIAL PRIMARY KEY,
                    Number TEXT UNIQUE NOT NULL,
                    Date TIMESTAMPTZ NOT NULL,
                    Note TEXT
                );";

        var createPositionTable = @"
                CREATE TABLE IF NOT EXISTS Positions (
                    Id SERIAL PRIMARY KEY,
                    WareId INT NOT NULL,
                    OrderId INT NOT NULL,
                    Quantity INT NOT NULL,
                    FOREIGN KEY (WareId) REFERENCES Wares(Id) ON DELETE CASCADE,
                    FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE
                );";
        
        var createEventsTable = @"
                CREATE TABLE IF NOT EXISTS Events (
                    Id SERIAL PRIMARY KEY,
                    Name TEXT NOT NULL,
                    Value INT NOT NULL,
                    Timestamp TIMESTAMP NOT NULL DEFAULT NOW()
                );";

        using var command = new NpgsqlCommand(createWareTable, connection);
        command.ExecuteNonQuery();

        command.CommandText = createOrderTable;
        command.ExecuteNonQuery();

        command.CommandText = createPositionTable;
        command.ExecuteNonQuery();
        
        command.CommandText = createEventsTable;
        command.ExecuteNonQuery();
    }
}
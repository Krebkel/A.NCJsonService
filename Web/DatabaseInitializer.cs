using Npgsql;

namespace Web
{
    public static class DatabaseInitializer
    {
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
                    FOREIGN KEY (WareId) REFERENCES Wares(Id) ON DELETE CASCADE,
                    FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE
                );";

            using var command = new NpgsqlCommand(createWareTable, connection);
            command.ExecuteNonQuery();

            command.CommandText = createOrderTable;
            command.ExecuteNonQuery();

            command.CommandText = createPositionTable;
            command.ExecuteNonQuery();
        }
    }
}

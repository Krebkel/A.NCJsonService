using Npgsql;

namespace Web;

public class Program
{
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        InitializeDatabase(host);

        host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });

    /// <summary>
    /// Первичная инициализация базы данных
    /// </summary>
    private static void InitializeDatabase(IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var configuration = services.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            // Создание таблиц
            DatabaseInitializer.CreateTables(connection);
        }
    }
}
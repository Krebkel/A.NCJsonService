using Dapper;
using Models;
using Npgsql;

namespace Web.Repositories;

public class WareRepository
{
    private readonly string _connectionString;

    public WareRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<IEnumerable<Ware>> GetWaresAsync()
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            var sql = "SELECT * FROM Wares";
            return await connection.QueryAsync<Ware>(sql);
        }
    }

    public async Task<Ware> GetWareByIdAsync(int id)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            var sql = "SELECT * FROM Wares WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Ware>(sql, new { Id = id });
        }
    }

    public async Task CreateWareAsync(Ware ware)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            var sql = "INSERT INTO Wares (Name, Value, Property) VALUES (@Name, @Value, @Property)";
            await connection.ExecuteAsync(sql, ware);
        }
    }

    public async Task UpdateWareAsync(Ware ware)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            var sql = "UPDATE Wares SET Name = @Name, Value = @Value, Property = @Property WHERE Id = @Id";
            await connection.ExecuteAsync(sql, ware);
        }
    }

    public async Task DeleteWareAsync(int id)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            var sql = "DELETE FROM Wares WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
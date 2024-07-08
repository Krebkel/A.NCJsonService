using Dapper;
using Models;
using Npgsql;

namespace Web.Repositories;

public class OrderRepository
{
    private readonly string _connectionString;

    public OrderRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync()
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            var sql = "SELECT * FROM Orders";
            return await connection.QueryAsync<Order>(sql);
        }
    }

    public async Task<Order> GetOrderByIdAsync(int id)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            var sql = "SELECT * FROM Orders WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Order>(sql, new { Id = id });
        }
    }

    public async Task CreateOrderAsync(Order order)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            var sql = "INSERT INTO Orders (Number, Date, Note) VALUES (@Number, @Date, @Note)";
            await connection.ExecuteAsync(sql, order);
        }
    }

    public async Task UpdateOrderAsync(Order order)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            var sql = "UPDATE Orders SET Number = @Number, Date = @Date, Note = @Note WHERE Id = @Id";
            await connection.ExecuteAsync(sql, order);
        }
    }

    public async Task DeleteOrderAsync(int id)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            var sql = "DELETE FROM Orders WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
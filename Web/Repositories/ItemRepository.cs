using Dapper;
using Models;
using Npgsql;

namespace Web.Repositories;

public class ItemRepository
{
    private readonly string _connectionString;

    public ItemRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<IEnumerable<Item>> GetItemsAsync()
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            var sql = @"SELECT i.*, w.*, o.* 
                        FROM Items i
                        JOIN Wares w ON i.WareId = w.Id
                        JOIN Orders o ON i.OrderId = o.Id";
            var items = await connection.QueryAsync<Item, Ware, Order, Item>(sql, (item, ware, order) =>
            {
                item.Ware = ware;
                item.Order = order;
                return item;
            }, splitOn: "Id,Id,Id");
            return items;
        }
    }

    public async Task<Item> GetItemByIdAsync(int id)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            var sql = @"SELECT i.*, w.*, o.*
                        FROM Items i
                        JOIN Wares w ON i.WareId = w.Id
                        JOIN Orders o ON i.OrderId = o.Id
                        WHERE i.Id = @Id";
            var items = await connection.QueryAsync<Item, Ware, Order, Item>(sql, (item, ware, order) =>
            {
                item.Ware = ware;
                item.Order = order;
                return item;
            }, new { Id = id }, splitOn: "Id,Id,Id");
            return items.FirstOrDefault();
        }
    }

    public async Task CreateItemAsync(Item item)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            var sql = "INSERT INTO Items (Number, WareId, OrderId) VALUES (@Number, @WareId, @OrderId)";
            await connection.ExecuteAsync(sql, new { item.Number, WareId = item.Ware.Id, OrderId = item.Order.Id });
        }
    }

    public async Task UpdateItemAsync(Item item)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            var sql = "UPDATE Items SET Number = @Number, WareId = @WareId, OrderId = @OrderId WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { item.Number, WareId = item.Ware.Id, OrderId = item.Order.Id, item.Id });
        }
    }

    public async Task DeleteItemAsync(int id)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            var sql = "DELETE FROM Items WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
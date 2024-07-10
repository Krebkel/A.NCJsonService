using System.Text;
using Dapper;
using Models.Accounting;
using Newtonsoft.Json;
using Npgsql;

namespace Web.Services.Accounting;

public class DataService
{
    private readonly NpgsqlConnection _connection;

    public DataService(NpgsqlConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    /// <summary>
    /// Вывод всех товаров
    /// </summary>
    public async Task<IEnumerable<Ware>> GetWaresAsync()
    {
        return await _connection.QueryAsync<Ware>("SELECT * FROM Wares");
    }

    /// <summary>
    /// Вывод товара по ID
    /// </summary>
    public async Task<Ware> GetWareByIdAsync(int id)
    {
        return await _connection.QueryFirstOrDefaultAsync<Ware>("SELECT * FROM Wares WHERE Id = @Id", new { Id = id });
    }

    /// <summary>
    /// Создать товар
    /// </summary>
    public async Task<int> CreateWareAsync(Ware ware)
    {
        var query = "INSERT INTO Wares (Name, Value, Property) VALUES (@Name, @Value, @Property) RETURNING Id";
        var wareId = await _connection.ExecuteScalarAsync<int>(query, ware);
        return wareId;
    }

    /// <summary>
    /// Обновить товар
    /// </summary>
    public async Task UpdateWareAsync(Ware ware)
    {
        await _connection.ExecuteAsync("UPDATE Wares SET Name = @Name, Value = @Value, Property = @Property WHERE Id = @Id", ware);
    }

    /// <summary>
    /// Удалить товар
    /// </summary>
    public async Task DeleteWareAsync(int id)
    {
        await _connection.ExecuteAsync("DELETE FROM Wares WHERE Id = @Id", new { Id = id });
    }

    /// <summary>
    /// Вывод всех заказов
    /// </summary>
    public async Task<IEnumerable<Order>> GetOrdersAsync()
    {
        var orders = await _connection.QueryAsync<Order>("SELECT * FROM Orders");
        return orders;
    }

    /// <summary>
    /// Вывод заказа по ID
    /// </summary>
    public async Task<Order> GetOrderByIdAsync(int id)
    {
        return await _connection.QueryFirstOrDefaultAsync<Order>("SELECT * FROM Orders WHERE Id = @Id", new { Id = id });
    }

    /// <summary>
    /// Создать заказ
    /// </summary>
    public async Task<int> CreateOrderAsync(Order order)
    {
        var query = "INSERT INTO Orders (Number, Date, Note) VALUES (@Number, @Date, @Note) RETURNING Id";
        var orderId = await _connection.ExecuteScalarAsync<int>(query, order);
        return orderId;
    }

    /// <summary>
    /// Обновить заказ
    /// </summary>
    public async Task UpdateOrderAsync(Order order)
    {
        await _connection.ExecuteAsync("UPDATE Orders SET Number = @Number, Date = @Date, Note = @Note WHERE Id = @Id", order);
    }

    /// <summary>
    /// Удалить заказ
    /// </summary>
    public async Task DeleteOrderAsync(int id)
    {
        await _connection.ExecuteAsync("DELETE FROM Orders WHERE Id = @Id", new { Id = id });
    }

    /// <summary>
    /// Вывод позиций заказа по ID заказа
    /// </summary>
    public async Task<IEnumerable<Position>> GetPositionsByOrderIdAsync(int orderId)
    {
        return await _connection.QueryAsync<Position>("SELECT * FROM Positions WHERE OrderId = @OrderId", new { OrderId = orderId });
    }

    /// <summary>
    /// Добавить позицию в заказ по ID заказа
    /// </summary>
    public async Task AddPositionToOrderAsync(Position position)
    {
        await _connection.ExecuteAsync("INSERT INTO Positions (WareId, OrderId) VALUES (@WareId, @OrderId)", position);
    }

    /// <summary>
    /// Убрать позицию заказа по ID заказа и ID товара
    /// </summary>
    public async Task RemovePositionFromOrderAsync(int orderId, int wareId)
    {
        await _connection.ExecuteAsync("DELETE FROM Positions WHERE OrderId = @OrderId AND WareId = @WareId", new { OrderId = orderId, WareId = wareId });
    }

    /// <summary>
    /// Импорт массива товаров из JSON
    /// </summary>
    public async Task ImportWaresFromJsonAsync(IFormFile importFile)
    {
        if (importFile == null || importFile.Length == 0)
        {
            throw new InvalidDataException("Invalid file.");
        }

        using var stream = new StreamReader(importFile.OpenReadStream());
        var content = await stream.ReadToEndAsync();
        var wares = JsonConvert.DeserializeObject<List<Ware>>(content);

        foreach (var ware in wares)
        {
            await CreateWareAsync(ware);
        }
    }

    /// <summary>
    /// Экспорт массива товаров в JSON
    /// </summary>
    public async Task<byte[]> ExportWaresToJsonAsync()
    {
        var wares = await GetWaresAsync();
        var json = JsonConvert.SerializeObject(wares);
        return Encoding.UTF8.GetBytes(json);
    }

    /// <summary>
    /// Импорт массива из одного товара из JSON по ID товара
    /// </summary>
    public async Task<byte[]> ExportWareByIdToJsonAsync(int id)
    {
        var ware = await GetWareByIdAsync(id);
        if (ware == null)
        {
            return null;
        }

        var wares = new List<Ware> { ware };
        var json = JsonConvert.SerializeObject(wares);
        return Encoding.UTF8.GetBytes(json);
    }

    /// <summary>
    /// Импорт массива заказов из JSON
    /// </summary>
    public async Task ImportOrdersFromJsonAsync(IFormFile importFile)
    {
        if (importFile == null || importFile.Length == 0)
        {
            throw new InvalidDataException("Invalid file.");
        }

        using var stream = new StreamReader(importFile.OpenReadStream());
        var content = await stream.ReadToEndAsync();

        var importDataList = JsonConvert.DeserializeObject<List<ImportOrderData>>(content);
    
        if (importDataList == null)
        {
            var singleImportData = JsonConvert.DeserializeObject<ImportOrderData>(content);
            if (singleImportData != null)
            {
                importDataList = new List<ImportOrderData> { singleImportData };
            }
        }

        if (importDataList == null)
        {
            throw new InvalidDataException("File does not contain valid order data.");
        }

        foreach (var orderData in importDataList)
        {
            var orderId = await CreateOrderAsync(orderData.Order);

            foreach (var position in orderData.Positions)
            {
                position.OrderId = orderId; 
                await AddPositionToOrderAsync(position);
            }
        }
    }

    /// <summary>
    /// Экспорт массива заказов в JSON
    /// </summary>
    public async Task<byte[]> ExportOrdersToJsonAsync()
    {
        var orders = await GetOrdersAsync();
        var ordersWithPositions = new List<ExportOrderData>();

        foreach (var order in orders)
        {
            var positions = await GetPositionsByOrderIdAsync(order.Id);
            ordersWithPositions.Add(new ExportOrderData
            {
                Order = order,
                Positions = positions.ToList()
            });
        }

        var json = JsonConvert.SerializeObject(ordersWithPositions);
        return Encoding.UTF8.GetBytes(json);
    }

    /// <summary>
    /// Импорт массива из одного заказа в JSON по ID заказа
    /// </summary>
    public async Task<byte[]> ExportOrderByIdToJsonAsync(int id)
    {
        var order = await GetOrderByIdAsync(id);
        if (order == null)
        {
            return null;
        }

        var positions = await GetPositionsByOrderIdAsync(order.Id);
        var orderWithPositions = new ExportOrderData
        {
            Order = order,
            Positions = positions.ToList()
        };

        var ordersWithPositions = new List<ExportOrderData> { orderWithPositions };
        var json = JsonConvert.SerializeObject(ordersWithPositions);
        return Encoding.UTF8.GetBytes(json);
    }

    /// <summary>
    /// Вспомогательный класс для импорта заказа
    /// </summary>
    public class ImportOrderData
    {
        public Order Order { get; set; }
        public List<Position> Positions { get; set; }
    }

    /// <summary>
    /// Вспомогательный класс для экспорта заказа
    /// </summary>
    public class ExportOrderData
    {
        public Order Order { get; set; }
        public List<Position> Positions { get; set; }
    }
}

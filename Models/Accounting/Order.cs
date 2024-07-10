namespace Models.Accounting;

/// <summary>
/// Заказ
/// </summary>
public class Order : DatabaseEntity
{
    /// <summary>
    /// Номер заказа
    /// </summary>
    public required string Number { get; set; }
    
    /// <summary>
    /// Дата создания заказа
    /// </summary>
    public required DateTimeOffset Date { get; set; }

    /// <summary>
    /// Примечание к заказу
    /// </summary>
    public string? Note { get; set; }
}
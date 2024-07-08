namespace Models;

/// <summary>
/// Товар в заказе
/// </summary>
public class Item : DatabaseEntity
{
    /// <summary>
    /// Номер позиции в заказе
    /// </summary>
    public required string Number { get; set; }
    
    /// <summary>
    /// Позиция
    /// </summary>
    public required Ware Ware { get; set; }
    
    /// <summary>
    /// Заказ
    /// </summary>
    public required Order Order { get; set; }
    
}
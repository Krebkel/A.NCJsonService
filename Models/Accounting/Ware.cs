namespace Models.Accounting;

/// <summary>
/// Товар
/// </summary>
public class Ware : DatabaseEntity
{
    /// <summary>
    /// Название товара
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Стоимость единицы товара
    /// </summary>
    public required decimal Value { get; set; }
    
    /// <summary>
    /// Свойство товара
    /// </summary>
    public string? Property { get; set; }
}
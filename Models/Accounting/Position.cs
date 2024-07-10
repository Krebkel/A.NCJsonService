namespace Models.Accounting;

public class Position : DatabaseEntity
{
 /// <summary>
    /// ID товара
    /// </summary>
    public required int WareId { get; set; }
    
    /// <summary>
    /// ID заказа
    /// </summary>
    public required int OrderId { get; set; }
}
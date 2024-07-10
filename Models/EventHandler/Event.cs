namespace Models.EventHandler;

public class Event : DatabaseEntity
{
    public string Name { get; set; }
    
    public int Value { get; set; }
    
    public DateTime Timestamp { get; set; }
    
}
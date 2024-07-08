using System.ComponentModel.DataAnnotations.Schema;

namespace Models;

/// <summary>
/// Общий класс для сущностей базы данных
/// </summary>
public abstract class DatabaseEntity
{
    /// <summary>
    /// Идентификатор сущности
    /// </summary>
    /// [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
}
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OlMag.Manufacture2.Models.Entities.SalesManager;

/// <summary>
/// Заказчик
/// </summary>
public class CustomerEntity
{
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Наименование организации
    /// </summary>
    [Required]
    public string Name { get; set; } = default!;

    /// <summary>
    /// Дата создания
    /// </summary>
    [Required]
    public DateTime Created { get; set; } = default!;

    /// <summary>
    /// Дата удаления (если есть связи)
    /// </summary>
    public DateTime? DateArchived { get; set; } = null;

    [Required]
    [Column(TypeName = "jsonb")]
    public CustomerInformationEntity Information { get; set; } = default!;
}

[NotMapped]
public class CustomerInformationEntity
{
    public string? Phone { get; set; }
    public string? Email { get; set; }
}
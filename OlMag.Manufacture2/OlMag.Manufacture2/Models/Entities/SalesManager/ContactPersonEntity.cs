using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OlMag.Manufacture2.Models.Entities.SalesManager;

/// <summary>
/// Контактное лицо
/// </summary>
public class ContactPersonEntity
{
    [Key] public Guid Id { get; set; }

    /// <summary>
    /// ФИО
    /// </summary>
    [Required]
    public string Name { get; set; } = default!;

    /// <summary>
    /// Заказчик
    /// </summary>
    public Guid CustomerId { get; set; } = default!;

    /// <summary>
    /// Заказчик
    /// </summary>
    public CustomerEntity Customer { get; set; } = default!;

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
    public ContactPersonInformationEntity Information { get; set; } = default!;
}

[NotMapped]
public class ContactPersonInformationEntity
{
    /// <summary>
    /// Должность
    /// </summary>
    public string Post { get; set; } = string.Empty;

    /// <summary>
    /// Телефон
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Почта
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Примечание
    /// </summary>
    public string? Note { get; set; }
}
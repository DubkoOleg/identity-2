using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OlMag.Manufacture2.Models.Entities.SaleManager;

public class CustomerEntity
{
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// ФИО
    /// </summary>
    [Required]
    public string Name { get; set; } = default!;

    [Required]
    [Column(TypeName = "jsonb")]
    public CustomerInformationEntity Information { get; set; } = default!;
}

[NotMapped]
public class CustomerInformationEntity
{
    /// <summary>
    /// Должность
    /// </summary>
    public string? Post { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}
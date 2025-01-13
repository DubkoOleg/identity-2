using System.ComponentModel.DataAnnotations;

namespace OlMag.Manufacture2.Models.Requests.SalesManager;

public class CustomerBodyRequest
{
    /// <summary>
    /// Наименование организации
    /// </summary>
    [Required]
    public string Name { get; set; } = default!;
    public string? Phone { get; set; }
    public string? Email { get; set; }
}
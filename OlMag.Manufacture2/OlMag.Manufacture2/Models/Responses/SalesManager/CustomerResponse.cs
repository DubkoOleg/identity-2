namespace OlMag.Manufacture2.Models.Responses.SalesManager;

public class CustomerResponse
{
    public Guid Id { get; set; }
    /// <summary>
    /// Наименование организации
    /// </summary>
    public string Name { get; set; } = default!;
    public string? Phone { get; set; }
    public string? Email { get; set; }
}
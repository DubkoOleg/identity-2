using System.ComponentModel.DataAnnotations;

namespace OlMag.Manufacture2.Models.Requests.SalesManager;

public class ContactPersonRequest: ContactPersonBodyRequest
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    [Required]
    public Guid Id { get; set; }
}
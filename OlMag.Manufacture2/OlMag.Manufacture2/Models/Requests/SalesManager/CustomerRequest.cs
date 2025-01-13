using System.ComponentModel.DataAnnotations;

namespace OlMag.Manufacture2.Models.Requests.SalesManager;

public class CustomerRequest : CustomerBodyRequest
{
    [Required]
    public Guid Id { get; set; }
}
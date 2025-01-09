using System.ComponentModel.DataAnnotations;

namespace OlMag.Manufacture2.Models.Requests
{
    public class RoleRequest
    {
        [Required]
        public string RoleName { get; set; } = default!;
    }
}

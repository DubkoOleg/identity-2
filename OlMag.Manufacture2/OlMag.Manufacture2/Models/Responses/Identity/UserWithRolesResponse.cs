namespace OlMag.Manufacture2.Models.Responses.Identity;

public class UserWithRolesResponse
{
    public string Id { get; set; } = default!;
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string[]? Roles { get; set; }
}

namespace OlMag.Manufacture2.Models.Responses.SalesManager;

public class ContactPersonInfoResponse : ContactPersonResponse
{
    public CustomerResponse Customer { get; set; } = default!;
}
namespace OlMag.Manufacture2.Models.Responses.SalesManager;

public class CustomerInfoResponse : CustomerResponse
{
    public ContactPersonResponse[] ContactPersons { get; set; } = default!;
}
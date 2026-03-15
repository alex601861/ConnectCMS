namespace CMSTrain.Client.Models.Responses.Country;

public class GetCountryDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Code { get; set; }

    public string PhoneCode { get; set; }

    public string Icon { get; set; }

    public bool IsActive {  get; set; } 
}

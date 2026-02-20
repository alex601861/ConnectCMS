namespace CMSTrain.Domain.Entities;

public class Country
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; }

    public string Code { get; set; }

    public string PhoneCode { get; set; }

    public string Icon { get; set; }

    public bool IsActive { get; set; }
}

namespace CMSTrain.Client.Models.Responses.Resource;

public class GetResourceDetailsDto
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string? Tag { get; set; }

    public string Description { get; set; }

    public string Type { get; set; }

    public bool IsLink { get; set; }

    public string? Link { get; set; }
}
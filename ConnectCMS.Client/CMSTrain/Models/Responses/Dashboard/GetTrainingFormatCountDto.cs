namespace CMSTrain.Client.Models.Responses.Dashboard;

public class GetTrainingFormatCountDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public int TotalCount { get; set; }
}
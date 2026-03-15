namespace CMSTrain.Client.Models.Responses.Inspection;

public class GetInspectionDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public string Type { get; set; }
    
    public int PhasesCount { get; set; }

    public bool IsActive { get; set; }
    
    public string? ImageUrl { get; set; }
    
    public bool HasAssignedQuestionnaire { get; set; }
}
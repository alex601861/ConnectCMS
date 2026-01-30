namespace CMSTrain.Application.DTOs.Dashboard;

public class GetAssignedTrainingDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string? ImageUrl { get; set; }

    public string NextClassDate { get; set; }
    
    public string NextClassTime { get; set; }

    public List<AssignedTrainersDto> Trainers { get; set; }

    public List<AssignedCandidatesDto> Candidates { get; set; }
}

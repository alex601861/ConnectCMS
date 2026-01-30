using CMSTrain.Application.DTOs.Training;

namespace CMSTrain.Application.DTOs.TrainingCandidate;

public class GetAllTrainingsForCandidate : GetTrainingDto
{
    public Guid? TrainingCandidateId { get; set; }
    
    public string Action { get; set; }
    
    public string? ActionDate { get; set; }

    public string? Remarks { get; set; }  
}
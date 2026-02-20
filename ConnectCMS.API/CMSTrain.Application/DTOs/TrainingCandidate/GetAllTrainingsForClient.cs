using CMSTrain.Application.DTOs.Training;

namespace CMSTrain.Application.DTOs.TrainingCandidate;

public class GetAllTrainingsForClient : GetTrainingDto
{
    public int CandidateCount { get; set; }
}
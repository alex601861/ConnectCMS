using CMSTrain.Client.Models.Responses.Training;

namespace CMSTrain.Client.Models.Responses.TrainingCandidate;

public class GetAllTrainingsForClient : GetTrainingDto
{
    public int CandidateCount { get; set; }
}
namespace CMSTrain.Client.Models.Responses.TrainingInspection;

public class GetTrainingInspectionQuestionnaireCountDto
{
    public int QuestionCount { get; set; }
    
    public int PossibleAnswerCount { get; set; }

    public int ResponseCount { get; set; }

    public int PendingAnalysisCount { get; set; }
}
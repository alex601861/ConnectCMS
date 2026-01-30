namespace CMSTrain.Application.DTOs.PersonalityTest;

public class GetPersonalityTestAnalysisDto
{
    public Guid UserResponseId { get; set; }
    
    public Guid QuestionnaireId { get; set; }
    
    public Guid TrainingInspectionId { get; set; }
    
    public Guid AnalysisId { get; set; }
    
    public List<PersonalityTestAnalysis> Analyses { get; set; }
    
    public List<PersonalityTestScore> Scores { get; set; }
}

public class PersonalityTestAnalysis : PersonalityTestTrait
{
    public string Description { get; set; }
    
    public List<PersonalityTestAnalysis>? Facets { get; set; }
}

public class PersonalityTestScore : PersonalityTestTrait
{
    public double Score { get; set; }
}
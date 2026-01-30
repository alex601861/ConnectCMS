using CMSTrain.Application.DTOs.Questionnaires;

namespace CMSTrain.Application.DTOs.PersonalityTest;

public class GetPersonalityTestQuestionnaireDto
{
    public Guid QuestionnaireId { get; set; }

    public Guid TrainingInspectionId { get; set; }
    
    public List<QuestionnaireTrait> QuestionnaireTraits { get; set; }
}

public class QuestionnaireTrait : PersonalityTestTrait
{
    public int QuestionCount { get; set; }
    
    public List<PersonalityTestFacet> Facets { get; set; }
}

public class PersonalityTestFacet
{
    public string Facet { get; set; }
    
    public string Description { get; set; }

    public List<GetQuestionDetailsDto> Questions { get; set; } = [];
}
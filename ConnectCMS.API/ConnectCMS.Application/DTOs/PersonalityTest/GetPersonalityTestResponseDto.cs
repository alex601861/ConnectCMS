using CMSTrain.Application.DTOs.Answer;

namespace CMSTrain.Application.DTOs.PersonalityTest;

public class GetPersonalityTestResponseDto
{
    public Guid UserResponseId { get; set; }
    
    public Guid QuestionnaireId { get; set; }

    public Guid TrainingInspectionId { get; set; }
    
    public int Phase { get; set; }

    public List<QuestionnaireResponseTrait> QuestionnaireTraits { get; set; }
}

public class QuestionnaireResponseTrait : PersonalityTestTrait
{
    public int QuestionCount { get; set; }
    
    public List<PersonalityTestResponseFacet> Facets { get; set; }
}

public class PersonalityTestResponseFacet
{
    public string Facet { get; set; }
    
    public string Description { get; set; }

    public List<GetQuestionAnswerDetailsDto> Questions { get; set; } = [];
}
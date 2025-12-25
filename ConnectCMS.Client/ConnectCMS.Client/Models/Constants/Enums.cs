using System.ComponentModel;

namespace CMSTrain.Client.Models.Constants;

public enum FileType
{
    [Description(".jpg,.png,.jpeg")] Image = 1,
    [Description(".mp4")] Video = 2,
    [Description(".mp3")] Audio = 3,
    [Description(".pdf,.xlsx,.doc")] Documents = 4,
    [Description(".com,.net,.org")] Link = 5,
    [Description(".com,.net,.org")] Post = 6,
}

public enum GenderType
{
    Male = 1,
    Female = 2,
    Other = 3
}

public enum InspectionType
{
    None = 0,
    SwotAnalysis = 1,
    PersonalityTest = 2,
    PersonalAssessment = 3,
    Feedback = 4,
    Others = 5
}

public enum SubordinateType
{
    Junior = 1,
    Peer = 2,
    Supervisor = 3,
}

public enum QuestionType
{
    None = 0,
    SingleSelectMcq = 1,
    MultiSelectMcq = 2,
    LongQuestion = 3,
    ShortQuestion = 4,
    Rating = 5
}

public enum StrategicType
{
    None = 0,
    Strength = 1,
    Weakness = 2,
    Opportunity = 3,
    Threat = 4
}

public enum HeadingType
{
    None = 0,
    Heading = 1,
    SubHeading = 2
}

public enum FacetType
{
    None = 0,
    Heading = 1,
    Facet = 2,
    Division = 3
}

public enum TraitType
{
    None = 0,
    Openness = 1,
    Conscientiousness = 2,
    Extraversion = 3,
    Agreeableness = 4,
    Neuroticism = 5
}

public enum TrainingConfiguration
{
    // ReSharper disable once InconsistentNaming
    RESOURCE_AVAILABILITY,
    // ReSharper disable once InconsistentNaming
    CERTIFICATIONS,
    // ReSharper disable once InconsistentNaming
    CERTIFICATION_TRIGGER
}

public enum ClassConfiguration
{
    // ReSharper disable once InconsistentNaming
    RESOURCE_AVAILABILITY,
    // ReSharper disable once InconsistentNaming
    ATTENDANCE_PERIOD
}

public enum TrainingInspectionConfiguration
{
    // ReSharper disable once InconsistentNaming
    RESPONSE_PERIOD
}
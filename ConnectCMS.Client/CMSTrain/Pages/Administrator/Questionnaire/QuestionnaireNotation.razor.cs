using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;

namespace CMSTrain.Client.Pages.Administrator.Questionnaire;

public partial class QuestionnaireNotation
{
    [Parameter] public string QuestionType { get; set; } = string.Empty;

    private QuestionType Type { get; set; } = Models.Constants.QuestionType.None;

    protected override void OnInitialized()
    {
        Type = Enum.Parse<QuestionType>(QuestionType);
    }
}
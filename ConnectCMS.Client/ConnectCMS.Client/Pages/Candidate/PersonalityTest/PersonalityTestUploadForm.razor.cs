using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Requests.PersonalityTest;
using CMSTrain.Client.Models.Responses.PersonalityTest;

namespace CMSTrain.Client.Pages.Candidate.PersonalityTest;

public partial class PersonalityTestUploadForm
{
    [Parameter] public string Trait { get; set; } = string.Empty;

    [Parameter] public List<PersonalityTestFacet> Facets { get; set; } = [];

    [Parameter] public List<PersonalityTestQuestionnaire> SelectedAnswers { get; set; } = [];
    
    [Parameter] public EventCallback OnAnswerSelected { get; set; }

    private async Task HandleAnswerSelection(Guid questionId, Guid selectedAnswerId)
    {
        var answer = SelectedAnswers.FirstOrDefault(a => a.QuestionId == questionId);
        
        if (answer != null)
        {
            answer.AnswerId = selectedAnswerId;
            
            await OnAnswerSelected.InvokeAsync();
        }
    }
}
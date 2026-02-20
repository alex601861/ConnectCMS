namespace CMSTrain.Application.DTOs.Questionnaires;

// Answers to be uploaded via Excel or Forms
public class McqAnswerDetailsDto
{
    public string Title { get; set; }
    
    public string? QuestionType { get; set; } 
}
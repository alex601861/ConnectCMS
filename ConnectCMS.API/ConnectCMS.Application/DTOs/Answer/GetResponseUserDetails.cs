using CMSTrain.Application.DTOs.Candidate;

namespace CMSTrain.Application.DTOs.Answer;

public class GetResponseUserDetails : GetCandidateDetailsDto
{
    public Guid UserResponseId { get; set; }
    
    public string AnsweredDate { get; set; }
}
using CMSTrain.Client.Models.Responses.Candidate;

namespace CMSTrain.Client.Models.Responses.Answers;

public class GetResponseUserDetails : GetCandidateDetailsDto
{
    public Guid UserResponseId { get; set; }
    
    public string AnsweredDate { get; set; }
}
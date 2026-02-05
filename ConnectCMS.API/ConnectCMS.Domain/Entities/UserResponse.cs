using CMSTrain.Domain.Common.Base;
using CMSTrain.Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMSTrain.Domain.Entities;

public class UserResponse : BaseEntity<Guid>
{
    [ForeignKey(nameof(Questionnaire))]
    public Guid QuestionId { get; set; }
    
    [ForeignKey(nameof(Candidate))]
    public Guid CandidateId { get; set; }

    [ForeignKey(nameof(Subordinate))]
    public Guid? SubordinateId { get; set; }

    public int Phase { get; set; }
    
    public bool IsAnsweredByCandidate { get; set; }

    public bool IsAnsweredBySubordinate { get; set; }
    
    public string? Remarks { get; set; }
    
    public DateTime AnsweredDate { get; set; }
    
    public virtual Questionnaire? Questionnaire { get; set; }
    
    public virtual User? Candidate { get; set; }
    
    public virtual Subordinate? Subordinate { get; set; }
    
    public virtual ICollection<UserResponseDetails>? UserResponseDetails { get; set; }
}


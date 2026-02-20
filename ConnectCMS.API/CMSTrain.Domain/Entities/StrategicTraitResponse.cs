using CMSTrain.Domain.Common.Base;
using CMSTrain.Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMSTrain.Domain.Entities;

public class StrategicTraitResponse : BaseEntity<Guid>
{
    [ForeignKey(nameof(Candidate))]
    public Guid CandidateId { get; set; }
    
    [ForeignKey(nameof(Questionnaire))]
    public Guid QuestionnaireId { get; set; }
    
    public int Phase { get; set; }
    
    public int Strengths { get; set; }
    
    public int Weaknesses { get; set; }
    
    public int Opportunities { get; set; } 
    
    public int Threats { get; set; }
    
    public virtual User? Candidate { get; set; }

    public virtual Questionnaire? Questionnaire { get; set; }
    
    public virtual List<StrategicTraitResponseDetails> StrategicTraitResponseDetails { get; set; }
}
using CMSTrain.Domain.Common.Base;
using System.ComponentModel.DataAnnotations.Schema;
using CMSTrain.Domain.Common.Enum;

namespace CMSTrain.Domain.Entities;

public class Answer : BaseEntity<Guid>
{
    public bool IsAnswerForInspection { get; set; }

    public bool IsAnswerForQuestion { get; set; }
    
    [ForeignKey(nameof(Inspection))]
    public Guid? InspectionId { get; set; }
    
    [ForeignKey(nameof(QuestionDetail))]
    public Guid? QuestionId { get; set; }

    public string Title { get; set; }

    public bool IsSelectable { get; set; }
    
    public int Order { get; set; }

    public QuestionType AnswerType { get; set; } = QuestionType.None;

    public virtual Inspection? Inspection { get; set; }

    public virtual QuestionnaireDetails? QuestionDetail { get; set; }
}

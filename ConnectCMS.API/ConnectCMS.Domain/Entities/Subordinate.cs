using CMSTrain.Domain.Common.Enum;
using CMSTrain.Domain.Common.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMSTrain.Domain.Entities;

public class Subordinate : BaseEntity<Guid>
{
    [ForeignKey(nameof(TrainingCandidate))]
    public Guid TrainingCandidateId { get; set; }

    public SubordinateType SubordinateType { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public string ContactNumber { get; set; }

    public virtual TrainingCandidate? TrainingCandidate { get; set; }
}

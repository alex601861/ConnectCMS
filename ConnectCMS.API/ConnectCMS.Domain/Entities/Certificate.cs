using CMSTrain.Domain.Common.Base;
using System.ComponentModel.DataAnnotations.Schema;
using CMSTrain.Domain.Common.Property;

namespace CMSTrain.Domain.Entities;

public class Certificate : BaseEntity<Guid>
{
    [ForeignKey(nameof(TrainingCandidate))]
    public Guid TrainingCandidateId { get; set; }

    public KeyValueProperty Description { get; set; }

    public KeyValueProperty Score {  get; set; }

    public string Remarks { get; set; }

    public virtual TrainingCandidate? TrainingCandidate { get; set; }
}

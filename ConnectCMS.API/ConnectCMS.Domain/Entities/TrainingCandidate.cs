using System.ComponentModel.DataAnnotations.Schema;
using CMSTrain.Domain.Common;
using CMSTrain.Domain.Common.Base;
using CMSTrain.Domain.Entities.Identity;

namespace CMSTrain.Domain.Entities;

public class TrainingCandidate : BaseEntity<Guid>
{
    [ForeignKey(nameof(Training))]
    public Guid TrainingId { get; set; }

    [ForeignKey(nameof(Candidate))]
    public Guid CandidateId { get; set; }

    public bool IsActionCompleted { get; set; }

    public bool IsApproved { get; set; }

    public string? Remarks { get; set; }

    public bool IsSelfRequested { get; set; }

    public bool IsOrganizationRequested { get; set; }
    
    public bool IsAdminRequested { get; set; }

    public bool IsOrganizationHandled { get; set; }

    public DateTime RequestedDate { get; set; }

    public DateTime? ActionDate { get; set; }

    public virtual Training? Training { get; set; }

    public virtual User? Candidate { get; set; }
}

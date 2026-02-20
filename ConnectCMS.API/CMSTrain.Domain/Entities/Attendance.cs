using CMSTrain.Domain.Common.Base;
using CMSTrain.Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMSTrain.Domain.Entities;

public class Attendance : BaseEntity<Guid>
{
    [ForeignKey(nameof(Class))]
    public Guid ClassId { get; set; }

    [ForeignKey(nameof(Candidate))]
    public Guid CandidateId { get; set; }

    public string AttendanceImageUrl { get; set; }

    public bool IsActionCompleted { get; set; }

    public bool IsApproved { get; set; }

    public string Remarks { get; set; } 

    public virtual Class? Class { get; set; }

    public virtual User? Candidate { get; set; }
}

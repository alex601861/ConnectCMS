using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using CMSTrain.Domain.Common.Enum;

namespace CMSTrain.Domain.Entities.Identity;

public class User : IdentityUser<Guid>
{
    public string Name { get; set; }

    public GenderType Gender { get; set; } 
    
    [ForeignKey(nameof(Designation))]
    public Guid? DesignationId { get; set; }
    
    public string? Address { get; set; }
    
    public string? ImageURL { get; set; }

    [ForeignKey(nameof(Country))]
    public Guid CountryId { get; set; }

    [ForeignKey(nameof(Organization))]
    public Guid? OrganizationId { get; set; }

    public DateTime RegisteredDate { get; set; } = DateTime.UtcNow;
    
    public bool IsActive { get; set; }

    public virtual Country? Country { get; set; }

    public virtual Designation Designation { get; set; }

    public virtual Organization? Organization { get; set; }

    public virtual ICollection<Training>? Trainings { get; set; }

    public virtual ICollection<TrainingCandidate>? TrainingCandidates { get; set; }
}
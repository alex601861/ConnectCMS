using CMSTrain.Domain.Common.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMSTrain.Domain.Entities;

public class Class : BaseEntity<Guid>
{
    public string Title { get; set; }
    
    public bool? IsDefaultClass { get; set; }
    
    [ForeignKey(nameof(Training))]
    public Guid TrainingId { get; set; }

    public string? ImageUrl { get; set; }
    
    public DateOnly Date { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public virtual Training? Training { get; set;}
    
    public virtual ICollection<ClassResources>? ClassResources { get; set; }
}

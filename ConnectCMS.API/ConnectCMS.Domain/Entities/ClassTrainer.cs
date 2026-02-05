using CMSTrain.Domain.Common.Base;
using CMSTrain.Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMSTrain.Domain.Entities;

public class ClassTrainer : BaseEntity<Guid>
{
    [ForeignKey(nameof(Class))]
    public Guid ClassId { get; set; }

    [ForeignKey(nameof(Trainer))]
    public Guid TrainerId { get; set; }

    public string? Description { get; set; }
    
    public virtual Class? Class { get; set; }

    public virtual User? Trainer { get; set; }
}

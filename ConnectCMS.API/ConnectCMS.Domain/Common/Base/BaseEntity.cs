using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMSTrain.Domain.Entities.Identity;

namespace CMSTrain.Domain.Common.Base;

public class BaseEntity<TPrimaryKey>
{
    [Key]
    public TPrimaryKey Id { get; set; }

    public bool IsActive { get; set; } = true;

    [ForeignKey(nameof(CreatedUser))]
    public Guid CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(LastModifiedUser))]
    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedAt { get; set; }

    [ForeignKey(nameof(DeletedUser))]
    public Guid? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual User? CreatedUser { get; set; }

    public virtual User? LastModifiedUser { get; set; }

    public virtual User? DeletedUser { get; set; }
}
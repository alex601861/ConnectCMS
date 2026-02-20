using CMSTrain.Domain.Common.Base;
using CMSTrain.Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMSTrain.Domain.Entities;

public class RoleRights : BaseEntity<Guid>
{
    [ForeignKey(nameof(Role))]
    public Guid RoleId { get; set; }

    [ForeignKey(nameof(Menu))]
    public Guid MenuId { get; set; }

    public virtual Role? Role { get; set; }

    public virtual Menu? Menu { get; set; }
}

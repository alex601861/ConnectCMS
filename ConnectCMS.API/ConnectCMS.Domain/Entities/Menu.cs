using CMSTrain.Domain.Common.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMSTrain.Domain.Entities;

public class Menu : BaseEntity<Guid>
{
    public string Title { get; set; }

    public string Description { get; set; }

    public int Sequence { get; set; }

    public string Url { get; set; }

    [ForeignKey(nameof(ParentMenuModule))]
    public Guid? ParentMenuId { get; set; }

    public virtual Menu? ParentMenuModule { get; set; }

    public virtual ICollection<Menu>? ChildMenus { get; set; }

    public virtual ICollection<RoleRights>? RoleRights { get; set; }
}

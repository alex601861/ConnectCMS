using CMSTrain.Domain.Common.Base;
using CMSTrain.Domain.Common.Property;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMSTrain.Domain.Entities;

public class UserResponseAnalysis : BaseEntity<Guid>
{
    [ForeignKey(nameof(UserResponse))]
    public Guid UserResponseId { get; set; }

    public string Title { get; set; }

    public KeyValueProperty Description { get; set; }

    public KeyValueProperty Scores { get; set; }

    public virtual UserResponse? UserResponse { get; set; }
}

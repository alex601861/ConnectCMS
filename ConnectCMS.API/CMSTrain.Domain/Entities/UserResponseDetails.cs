using CMSTrain.Domain.Common.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMSTrain.Domain.Entities;

public class UserResponseDetails : BaseEntity<Guid>
{
    [ForeignKey(nameof(UserResponse))]
    public Guid UserResponseId { get; set; }

    [ForeignKey(nameof(Answer))]
    public Guid AnswerId { get; set; }

    public virtual UserResponse UserResponse { get; set; }

    public virtual Answer Answer { get; set; }
}
using CMSTrain.Domain.Common.Base;
using CMSTrain.Domain.Common.Enum;

namespace CMSTrain.Domain.Entities;

public class Resource : BaseEntity<Guid>
{
    public string Title { get; set; }

    public string Description { get; set; }

    public string Tag { get; set; }

    public string FileName { get; set; }

    public string FileUrl { get; set; }

    public FileType Type { get; set; }
}
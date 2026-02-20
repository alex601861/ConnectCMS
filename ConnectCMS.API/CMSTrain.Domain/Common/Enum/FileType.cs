using System.ComponentModel;

namespace CMSTrain.Domain.Common.Enum;

public enum FileType
{
    [Description(".api")] None = 0,
    [Description(".jpg,.png,.jpeg,.gif,.svg")] Image = 1,
    [Description(".mp4")] Video = 2,
    [Description(".mp3")] Audio = 3,
    [Description(".pdf,.xlsx,.doc")] Documents = 4,
    [Description(".com,.net,.org")] Link = 5,
    [Description(".com,.net,.org")] Post = 6,
}
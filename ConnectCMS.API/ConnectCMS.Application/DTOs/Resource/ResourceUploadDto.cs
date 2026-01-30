using Microsoft.AspNetCore.Http;
using CMSTrain.Application.Common.Attributes;

namespace CMSTrain.Application.DTOs.Resource;

public class ResourceUploadDto
{
    public string Title { get; set; }

    public bool IsLink { get; set; }

    public string Description { get; set; }

    public string? Link { get; set; }

    [FileExamination(5 * 1024 * 1024, true)]
    public IFormFile? ResourceFile { get; set; }
}

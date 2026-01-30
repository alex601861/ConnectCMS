using CMSTrain.Application.Common.Attributes;
using Microsoft.AspNetCore.Http;

namespace CMSTrain.Application.DTOs.Resource;

public class UpdateResourceDto : ResourceUploadDto
{
    public Guid Id { get; set; }
}
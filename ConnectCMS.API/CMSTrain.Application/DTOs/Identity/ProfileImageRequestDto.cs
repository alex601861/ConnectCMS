using Microsoft.AspNetCore.Http;
using CMSTrain.Application.Common.Attributes;

namespace CMSTrain.Application.DTOs.Identity;

public class ProfileImageRequestDto
{
    [FileExamination(5 * 1024 * 1024)]
    public IFormFile ImageUrl { get; set; }
}
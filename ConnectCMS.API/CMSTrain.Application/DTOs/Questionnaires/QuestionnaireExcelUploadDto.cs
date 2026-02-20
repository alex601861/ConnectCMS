using CMSTrain.Application.Common.Attributes;
using Microsoft.AspNetCore.Http;

namespace CMSTrain.Application.DTOs.Questionnaires;

public class QuestionnaireExcelUploadDto : QuestionnaireDto
{
    [FileExamination(5 * 1024 * 1024)]
    public IFormFile File { get; set; }
}

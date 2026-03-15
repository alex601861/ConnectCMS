using Microsoft.AspNetCore.Components.Forms;

namespace CMSTrain.Client.Models.Requests.Questionnaires;

public class QuestionnaireExcelUploadDto : QuestionnaireDto
{
    public IBrowserFile File { get; set; }
}

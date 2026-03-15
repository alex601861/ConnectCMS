namespace CMSTrain.Client.Models.Responses.Heading;

public class GetHeadingDto : GetHeadingModuleDto
{
    public List<GetHeadingModuleDto> SubHeadings { get; set; } = [];
}
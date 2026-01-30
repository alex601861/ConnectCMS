namespace CMSTrain.Application.DTOs.Heading;

public class GetHeadingDto : GetHeadingModuleDto
{
    public List<GetHeadingModuleDto> SubHeadings { get; set; } = [];
}
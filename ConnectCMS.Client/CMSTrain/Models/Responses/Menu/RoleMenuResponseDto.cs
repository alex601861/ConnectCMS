namespace CMSTrain.Client.Models.Responses.Menu;

public class RoleMenuResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public int Sequence { get; set; }

    public string Url { get; set; }

    public bool IsAssigned { get; set; }

    public List<RoleMenuResponseDto>? SubMenus { get; set; } = [];
}
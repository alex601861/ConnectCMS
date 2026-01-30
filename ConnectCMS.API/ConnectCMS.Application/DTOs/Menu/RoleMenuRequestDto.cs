namespace CMSTrain.Application.DTOs.Menu;

public class RoleMenuRequestDto
{
    public Guid RoleId { get; set; }

    public List<Guid> MenuIds { get; set; }
}
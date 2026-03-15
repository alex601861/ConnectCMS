namespace CMSTrain.Client.Models.Requests.Menu;

public class RoleMenuRequestDto
{
    public Guid RoleId { get; set; }

    public List<Guid> MenuIds { get; set; }
}
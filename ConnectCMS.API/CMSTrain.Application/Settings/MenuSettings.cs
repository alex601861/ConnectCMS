namespace CMSTrain.Application.Settings;

public class MenuSettings
{
    public List<MenuConfiguration> Menus { get; set; }
}

public class MenuConfiguration
{
    public string Id { get; set; }
    
    public string Title { get; set; }

    public string Description { get; set; }
    
    public int Sequence { get; set; }
    
    public string Url { get; set; }
    
    public string Icon { get; set; }
    
    public bool IsActive { get; set; }
    
    public List<MenuConfiguration>? ChildMenus { get; set; }
}
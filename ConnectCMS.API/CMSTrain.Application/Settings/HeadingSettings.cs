namespace CMSTrain.Application.Settings;

public class HeadingSettings
{
    public List<HeadingConfiguration> Headings { get; set; }
}

public class HeadingConfiguration
{
    public string Title { get; set; }

    public string Facet { get; set; }

    public string Inspection { get; set; }
    
    public List<HeadingConfiguration>? SubHeadings { get; set; }
}

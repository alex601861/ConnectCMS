namespace CMSTrain.Application.Settings;

public class TraitSettings
{
    public List<TraitConfiguration> PersonalityTraits { get; set; }
}

public class TraitConfiguration
{
    public string Title { get; set; }

    public string Type { get; set; }
}

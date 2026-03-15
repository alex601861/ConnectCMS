namespace CMSTrain.Client.Models.Responses.Location;

public class LocationResultDto
{
    public bool IsWithinRadius { get; set; }
    
    public double Distance { get; set; }
}
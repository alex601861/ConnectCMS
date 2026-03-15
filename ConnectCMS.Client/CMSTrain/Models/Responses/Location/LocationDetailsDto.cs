namespace CMSTrain.Client.Models.Responses.Location;

public class LocationDetailsDto
{
    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public double? Altitude { get; set; } = 0.00;

    public double? Accuracy { get; set; } = 0.00;

    public double? AltitudeAccuracy { get; set; } = 0.00;

    public double? Heading { get; set; } = 0.00;

    public double? Speed { get; set; } = 0.00;
}
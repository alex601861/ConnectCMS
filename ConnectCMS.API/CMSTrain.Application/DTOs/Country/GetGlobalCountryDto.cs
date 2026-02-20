namespace CMSTrain.Application.DTOs.Country;

public class GetGlobalCountryDto
{
    public string Status { get; set; }
    
    public string Version { get; set; }
    
    public string Access { get; set; }
    
    public int Total { get; set; }
    
    public int Offset { get; set; }
    
    public int Limit { get; set; }
    
    public Dictionary<string, CountryInfo> Data { get; set; }
}

public class CountryInfo
{
    public string Country { get; set; }
    
    public string Region { get; set; }
}
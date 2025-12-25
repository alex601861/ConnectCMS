using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.Location;
using MudBlazor.Utilities;

namespace CMSTrain.Client.Service.Extensions;

public static class ExtensionMethods
{
    public static DateTime ToDateTimeFromDateOnlyString(this string dateString)
    {
        var dateTime = DateTime.ParseExact(dateString, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
    
        return dateTime;
    }
    
    public static DateTime ToDateTimeFromDateTimeString(this string dateString)
    {
        var dateTime = DateTime.ParseExact(dateString, "dd.MM.yyyy h:mm tt", System.Globalization.CultureInfo.InvariantCulture);
    
        return dateTime;
    }
    
    public static TimeSpan ToTimeSpanFromTimeSpanString(this string timeString)
    {
        if (DateTime.TryParseExact(timeString, "hh:mm tt", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var dateTime))
        {
            return dateTime.TimeOfDay;
        }

        throw new FormatException("Input string was not in a correct format.");
    }
    
    public static string GetDayWithSuffix(int day)
    {
        if (day % 100 >= 11 && day % 100 <= 13) return day + "th";
        var value = day % 10; 
        return value  switch
        {
            1 => day + "st",
            2 => day + "nd",
            3 => day + "rd",
            _ => day + "th",
        };
    }
    
    public static Guid? ToNullOrValue(Guid? moduleId)
    {
        if (moduleId == Guid.Empty || moduleId == null)
        {
            return null;
        }

        return moduleId;
    }

    public static InspectionType ToInspectionTypeString(this string inspectionTypeString)
    {
        return inspectionTypeString switch
        {
            "Personality Test" => InspectionType.PersonalityTest,
            "SWOT Analysis" => InspectionType.SwotAnalysis,
            "Feedback" => InspectionType.Feedback,
            "Personal Assessment" => InspectionType.PersonalAssessment,
            "Other" => InspectionType.Others,
            null => InspectionType.None,
            _ => throw new ArgumentException($"Unknown inspection type: {inspectionTypeString}")
        };
    }
    
    public static LocationResultDto IsWithinActualRadius(double actualLatitude, double actualLongitude, double positionLatitude, double positionLongitude, double radius)
    {
        const double earthRadiusKm = 6371;

        var actualLatitudeRadian = ToRadians(actualLatitude);
        var actualLongitudeRadian = ToRadians(actualLongitude);
        var positionLatitudeRadian = ToRadians(positionLatitude);
        var positionLongitudeRadian = ToRadians(positionLongitude);
    
        // Differences in Coordinates
        var latitudeDifference = positionLatitudeRadian - actualLatitudeRadian;
        var longitudeDifference = positionLongitudeRadian - actualLongitudeRadian;
    
        // Haversine Formula
        var haversineSquareHalf = Math.Sin(latitudeDifference/2) * Math.Sin(latitudeDifference/2) +
                                  Math.Cos(actualLatitudeRadian) * Math.Cos(positionLatitudeRadian) *
                                  Math.Sin(longitudeDifference/2) * Math.Sin(longitudeDifference/2);
    
        var angularDistance = 2 * Math.Atan2(Math.Sqrt(haversineSquareHalf), Math.Sqrt(1-haversineSquareHalf));
        
        var distanceKm = earthRadiusKm * angularDistance; // Distance in kilometers
    
        return new LocationResultDto
        {
            IsWithinRadius = distanceKm <= radius,
            Distance = Math.Round(distanceKm, 2)
        };
    }
    
    public static MudColor ToMudColor(this string hexCode)
    {
        if (string.IsNullOrEmpty(hexCode))
            throw new ArgumentException("Invalid hexCode code.");

        hexCode = hexCode.TrimStart('#');

        switch (hexCode.Length)
        {
            case 6:
            {
                int r = Convert.ToByte(hexCode[..2], 16);
                int g = Convert.ToByte(hexCode.Substring(2, 2), 16);
                int b = Convert.ToByte(hexCode.Substring(4, 2), 16);
                return new MudColor(r, g, b, 255);
            }
            case 8:
            {
                int r = Convert.ToByte(hexCode[..2], 16);
                int g = Convert.ToByte(hexCode.Substring(2, 2), 16);
                int b = Convert.ToByte(hexCode.Substring(4, 2), 16);
                int a = Convert.ToByte(hexCode.Substring(6, 2), 16);
                return new MudColor(r, g, b, a);
            }
            default:
                throw new ArgumentException("Hex code must be 6 or 8 characters long.");
        }
    }
    
    public static string ToHexCode(this MudColor color)
    {
        return color.ToString(MudColorOutputFormats.Hex);
    }
    
    private static double ToRadians(double degrees)
    {
        return degrees * (Math.PI/180);
    }
}
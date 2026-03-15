namespace CMSTrain.Client.Models.Responses.Dashboard;

public class GetTrainingProgressDto
{
    public int TotalRegisteredCandidates { get; set; }
    
    public double? TotalRegisteredCandidatesGrowth { get; set; }

    public int AttendedClasses { get; set; }

    public double? AttendedClassesGrowth { get; set; }

    public int TotalPossibleAttendances { get; set; }

    public double? TotalPossibleAttendancesGrowth { get; set; }

    public int TrainingsInProgress { get; set; }

    public double? TrainingsInProgressGrowth { get; set; }

    public int TrainingsCompleted { get; set; }

    public double? TrainingsCompletedGrowth { get; set; }

    public int CertificationsEarned { get; set; }

    public double? CertificationsEarnedGrowth { get; set; }
}
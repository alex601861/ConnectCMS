namespace CMSTrain.Application.DTOs.Dashboard;

public class GetTrainingRequestsSummaryDto
{
    public List<TrainingStartDateSummary> FirstTraining { get; set; }
    
    public List<TrainingStartDateSummary> SecondTraining { get; set; }
    
    public List<TrainingStartDateSummary> ThirdTraining { get; set; }

    public List<TrainingStartDateSummary> FourthTraining { get; set; }
    
    public List<TrainingStartDateSummary> FifthTraining { get; set; }
}

public class TrainingStartDateSummary
{
    public string Month { get; set; }
    
    public double? Date { get; set; }
    
    public string Title { get; set; }
}
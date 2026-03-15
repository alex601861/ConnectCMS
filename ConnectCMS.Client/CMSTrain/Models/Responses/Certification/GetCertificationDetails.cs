namespace CMSTrain.Client.Models.Responses.Certification;

public class GetCertificationDetails
{
    public Guid Id { get; set; }
    
    public Guid TrainingId { get; set; }
    
    public Guid TrainingCandidateId { get; set; }

    public CertificationDetails CertificationDetails { get; set; } = new();
}

public class CertificationDetails
{
    public string Training { get; set; } = "Name of the Training";

    public string Candidate { get; set; } = "Name of the Candidate";

    public string Date { get; set; } = "Date of Completion";
}
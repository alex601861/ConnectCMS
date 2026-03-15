namespace CMSTrain.Client.Models.Requests.Designation;

public class UpdateDesignationDto
{
    public Guid Id { get; set; } 
    
    public string Title { get; set; }
    
    public string Description { get; set; }
}
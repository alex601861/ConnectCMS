namespace CMSTrain.Application.DTOs.Designation;

public class GetDesignationDto
{
    public Guid Id { get; set; }
    
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public bool IsActive { get; set; }
}
namespace CMSTrain.Domain.Entities;

public class Designation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public bool IsActive { get; set; }
}
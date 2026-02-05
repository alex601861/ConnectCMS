using CMSTrain.Domain.Common.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMSTrain.Domain.Entities;

public class Training : BaseEntity<Guid>
{
    public string Title { get; set; }

    public string Description { get; set; }

    public string LocationDetails { get; set; }
    
    public decimal? Longitude { get; set; }

    public decimal? Latitude { get; set; }

    [ForeignKey(nameof(TrainingFormat))]
    public Guid TrainingFormatId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string? ImageUrl { get; set; }

    public virtual TrainingFormat? TrainingFormat { get; set; }
    
    public virtual ICollection<Class>? Classes { get; set; }

    public virtual ICollection<TrainingResources>? TrainingResources { get; set; }
}

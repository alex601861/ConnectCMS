namespace CMSTrain.Client.Models.Responses.ClassTrainers;

public class GetTrainersDto
{
    public Guid Id { get; set; }

    public string? ImageUrl { get; set; }

    public string Name { get; set; }

    public string Username { get; set; }

    public string EmailAddress { get; set; }

    public string PhoneNumber { get; set; }
}
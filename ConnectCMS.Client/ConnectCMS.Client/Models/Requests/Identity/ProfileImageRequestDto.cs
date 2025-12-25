using Microsoft.AspNetCore.Components.Forms;

namespace CMSTrain.Client.Models.Requests.Identity;

public class ProfileImageRequestDto
{
    public IBrowserFile ImageUrl { get; set; }
}
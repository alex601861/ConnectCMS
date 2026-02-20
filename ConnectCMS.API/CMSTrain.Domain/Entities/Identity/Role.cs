using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMSTrain.Domain.Entities.Identity;

public class Role : IdentityRole<Guid>;
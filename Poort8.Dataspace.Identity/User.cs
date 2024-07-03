using Microsoft.AspNetCore.Identity;

namespace Poort8.Dataspace.Identity;

public class User : IdentityUser
{
    public string IshareId { get; set; } = "N/A";
}

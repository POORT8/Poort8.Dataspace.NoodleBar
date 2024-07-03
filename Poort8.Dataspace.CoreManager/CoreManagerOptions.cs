using System.ComponentModel.DataAnnotations;

namespace Poort8.Dataspace.CoreManager;

public class CoreManagerOptions
{
    public const string Section = "CoreManagerOptions";

    [Required]
    public required string UseCase { get; set; }
    public string EmployeeAlternativeNamePlural { get; set; } = "Employees";
    public string EmployeeAlternativeName { get; set; } = "Employee";
    public string ResourceGroupAlternativeNamePlural { get; set; } = "ResourceGroups";
    public string ResourceGroupAlternativeName { get; set; } = "ResourceGroup";
    public string ResourceAlternativeNamePlural { get; set; } = "Resources";
    public string ResourceAlternativeName { get; set; } = "Resource";
    public string? JwtTokenAuthority { get; set; }
    public string? JwtTokenAudience { get; set; }
}

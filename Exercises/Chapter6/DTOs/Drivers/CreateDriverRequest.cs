using System.ComponentModel.DataAnnotations;
namespace Chapter6.DTOs.Drivers;
public class CreateDriverRequest
{
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;
}
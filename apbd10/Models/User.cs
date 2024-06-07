using System.ComponentModel.DataAnnotations;

namespace apbd10.Models;

public class User
{
    [EmailAddress]
    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = null!;
    [Required]
    [MaxLength(100)]
    public string Login { get; set; } = null!;
    [Required]
    [MaxLength(100)]
    public string HashedPassword { get; set; } = null!;
    [Required]
    [MaxLength(100)]
    public string Salt { get; set; } = null!;
    [MaxLength(100)]
    [Required]
    public string RefreshToken { get; set; } = null!;
}
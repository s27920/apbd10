using System.ComponentModel.DataAnnotations;

namespace AuthExample.Dtos.User;

public class RefreshTokenRequestDto
{
    [Required]
    public string AccessToken { get; set; }
    [Required]
    public string RefreshToken { get; set; }
}
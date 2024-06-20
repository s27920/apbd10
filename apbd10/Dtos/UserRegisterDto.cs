namespace apbd10.Dtos;

public class UserRegisterDto
{
    public string Email { get; set; } = null!;
    public string Login { get; set; } = null!;
    public string HashedPassword { get; set; } = null!;
    public string? Salt { get; set; }
}
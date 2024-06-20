using System.Security.Cryptography;
using System.Text;
using apbd10.Dtos;
using apbd10.Exceptions;
using apbd10.Repositories;
using AuthExample.Dtos.User;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace apbd10.Services;

public interface IUserService
{
    public Task<bool> RegisterUser(UserRegisterDto dto);
    public Task<TokenResponseDto> Login(UserLoginDto dto);
    public Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto);
    public Tuple<string, string> HashAndSaltPassword(string password, byte[] salt);
    public byte[] CreateSalt();

}

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> RegisterUser(UserRegisterDto dto)
    {
        if (!await _userRepository.CheckEmailAvailabilityAsync(dto.Email))
        {
            throw new ConflictException("User with the provided email address already exists");
        }
        Tuple<string, string> saltAndPassword = HashAndSaltPassword(dto.HashedPassword, CreateSalt());
        dto.HashedPassword = saltAndPassword.Item1;
        dto.Salt = saltAndPassword.Item2;
        return await _userRepository.RegisterUser(dto);
    }

    public byte[] CreateSalt()
    {
        byte[] salt = new byte[128 / 8];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);
        return salt;
    }

    public async Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto)
    {
        return await _userRepository.RefreshTokenAsync(dto);
    }

    public Tuple<string, string> HashAndSaltPassword(string password, byte[] salt)
    {
        
        string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(password: password, salt: salt,
            prf: KeyDerivationPrf.HMACSHA1, iterationCount: 10000, numBytesRequested: 256 / 8));
        string saltBase64 = Convert.ToBase64String(salt);
        return new Tuple<string, string>(hashedPassword, saltBase64);
    }

    public async Task<TokenResponseDto> Login(UserLoginDto dto)
    {
        if (await _userRepository.CheckEmailAvailabilityAsync(dto.Email))
        {
            throw new ConflictException("Unauthorized");
        }
        dto.password = HashAndSaltPassword(dto.password, Encoding.UTF8.GetBytes(
            await  _userRepository.GetSaltAsync(dto.Email)
            )).Item1;
        return await _userRepository.Login(dto);
    }
}
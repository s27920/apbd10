using System.Security.Cryptography;
using apbd10.Dtos;
using apbd10.Repositories;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace apbd10.Services;

public interface IUserService
{
    public Task<bool> RegisterUser(UserRegisterDto dto);
    public Tuple<string, string> HashAndSaltPassword(string password);
}

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Task<bool> RegisterUser(UserRegisterDto dto)
    {
        Tuple<string, string> saltAndPassword = HashAndSaltPassword(dto.HashedPassword);
        dto.HashedPassword = saltAndPassword.Item1;
        dto.Salt = saltAndPassword.Item2;
        return _userRepository.RegisterUser(dto);
    }

    public Tuple<string, string> HashAndSaltPassword(string password)
    {
        byte[] salt = new byte[128 / 8];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);
        string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(password: password, salt: salt,
            prf: KeyDerivationPrf.HMACSHA1, iterationCount: 10000, numBytesRequested: 256 / 8));
        string saltBase64 = Convert.ToBase64String(salt);
        return new Tuple<string, string>(hashedPassword, saltBase64);
    }
    
}
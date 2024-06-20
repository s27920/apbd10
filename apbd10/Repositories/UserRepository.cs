using System.Security.Claims;
using apbd10.Context;
using apbd10.Dtos;
using apbd10.Exceptions;
using apbd10.Models;
using apbd10.Services;
using AuthExample.Dtos.User;
using Microsoft.EntityFrameworkCore;

namespace apbd10.Repositories;

public interface IUserRepository
{
    public Task<bool> RegisterUser(UserRegisterDto dto);
    public Task<bool> CheckEmailAvailabilityAsync(String email);
    public Task<TokenResponseDto> Login(UserLoginDto dto);
    public Task<string> GetSaltAsync(string login);
    public Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto);
}

public class UserRepository : IUserRepository
{
    private readonly UserContext _context;
    private ITokenService _tokenService;

    public UserRepository(UserContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<bool> RegisterUser(UserRegisterDto dto)
    {
        int newId = _context.Users.Max(user => user.UserId);
        await _context.Users.AddAsync(new User
        {
            UserId = newId+1,
            Email = dto.Email,
            HashedPassword = dto.HashedPassword,
            Login = dto.Login,
            Salt = dto.Salt,
            RefreshToken = null
        });
        return true;
    }

    public async Task<TokenResponseDto> Login(UserLoginDto dto)
    {
        var user = await _context.Users.FirstAsync(user => user.Email.Equals(dto.Email, StringComparison.CurrentCultureIgnoreCase));
        if (!dto.password.Equals(user.HashedPassword))
        {
            throw new ConflictException("Unauthorized");
        }

        var refreshToken = _tokenService.GenerateRefreshToken();
        user.RefreshToken = refreshToken.token;
        user.RefreshTokenExpiration = refreshToken.expiration;
        return new TokenResponseDto
        {
            AccessToken = _tokenService.GenerateAccessToken(user),
            RefreshToken = refreshToken.token
        };
    }

    public async Task<bool> CheckEmailAvailabilityAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(user => user.Email.Equals(email)) == null;
    }

    public async Task<string> GetSaltAsync(string login)
    {
        var user = await _context.Users.FirstAsync(user => user.Login.Equals(login, StringComparison.CurrentCultureIgnoreCase));
        return user.Login;
    }

    public async Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto)
    {
        var principal = _tokenService.ValidateAndGetPrincipalFromJwt(dto.AccessToken, false);
        if (principal == null)
        {
            throw new ConflictException("Unauthorized");
        }
        var claimIdUser = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (claimIdUser is null || !int.TryParse(claimIdUser, out _))
        {
            throw new ConflictException("Unauthorized");
        }
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == int.Parse(claimIdUser));
        if (user is null || user.RefreshToken != dto.RefreshToken || user.RefreshTokenExpiration < DateTime.UtcNow)
        {
            throw new ConflictException("Unauthorized");
        }
        var refreshToken = _tokenService.GenerateRefreshToken();
        user.RefreshToken = refreshToken.token;
        user.RefreshTokenExpiration = refreshToken.expiration;
        
        return new TokenResponseDto
        {
            AccessToken = _tokenService.GenerateAccessToken(user),
            RefreshToken = refreshToken.token,
        };
    }
}
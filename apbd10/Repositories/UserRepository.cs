using apbd10.Dtos;
using Microsoft.EntityFrameworkCore;

namespace apbd10.Repositories;

public interface IUserRepository
{
    public Task<bool> RegisterUser(UserRegisterDto dto);
}

public class UserRepository : IUserRepository
{
    private readonly DbContext _context;

    public UserRepository(DbContext context)
    {
        _context = context;
    }

    public Task<bool> RegisterUser(UserRegisterDto dto)
    {
        throw new NotImplementedException();
    }
}
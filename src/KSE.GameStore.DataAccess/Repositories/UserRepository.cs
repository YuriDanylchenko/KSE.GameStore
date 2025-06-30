using KSE.GameStore.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace KSE.GameStore.DataAccess.Repositories;

public class UserRepository(GameStoreDbContext context) : Repository<User, Guid>(context), IUserRepository
{
    public Task<User?> GetUserByEmailWithRoles(string email, CancellationToken ct)
    {
        return _context.Users
            .Include(x => x.UserRoles).ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken: ct);
    }
}
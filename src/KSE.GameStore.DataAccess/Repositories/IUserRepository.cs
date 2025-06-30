using KSE.GameStore.DataAccess.Entities;

namespace KSE.GameStore.DataAccess.Repositories;

public interface IUserRepository : IRepository<User, Guid>
{
    Task<User?> GetUserByEmailWithRoles(string email, CancellationToken ct);
}
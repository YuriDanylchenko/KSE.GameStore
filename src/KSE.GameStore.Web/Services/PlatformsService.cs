using KSE.GameStore.DataAccess;
using KSE.GameStore.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace KSE.GameStore.Web.Services
{
    public class PlatformsService(GameStoreDbContext dbContext)
    {
        public async Task<List<Platform>> GetAllAsync()
        {
            return await dbContext.Platforms.ToListAsync();
        }

        public async Task<Platform?> GetByIdAsync(int id)
        {
            return await dbContext.Platforms.FindAsync(id);
        }

        public async Task<Platform> CreateAsync(Platform platform)
        {
            dbContext.Platforms.Add(platform);
            await dbContext.SaveChangesAsync();
            return platform;
        }

        public async Task<bool> UpdateAsync(int id, Platform platform)
        {
            var existing = await dbContext.Platforms.FindAsync(id);
            if (existing == null) return false;
            existing.Name = platform.Name;
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await dbContext.Platforms.FindAsync(id);
            if (existing == null) return false;
            dbContext.Platforms.Remove(existing);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}
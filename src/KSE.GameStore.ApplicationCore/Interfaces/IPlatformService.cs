using KSE.GameStore.ApplicationCore.Domain;

namespace KSE.GameStore.ApplicationCore.Interfaces;

/// <summary>
/// Defines methods for managing platform entities, including retrieval, creation, updating, and deletion operations.
/// </summary>
public interface IPlatformsService
{
    /// <summary>
    /// Retrieves all platforms from the data store.
    /// </summary>
    /// <returns></returns>
    Task<List<PlatformDto>> GetAllAsync();
    /// <summary>
    /// Retrieves a platform by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the platform.</param>
    /// <returns>A task that represents the asynchronous operation, containing the platform if found or null if not.</returns>
    Task<PlatformDto> GetByIdAsync(int id);
    /// <summary>
    /// Creates a new platform with the specified name.
    /// </summary>
    /// <param name="name">The name of the platform to create.</param>
    /// <returns>A task that represents the asynchronous operation, containing the unique identifier of the created platform.</returns>
    Task<int> CreateAsync(string name);
    /// <summary>
    /// Updates an existing platform's name by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the platform to update.</param>
    /// <param name="name">The new name for the platform.</param>
    /// <returns>A task that represents the asynchronous operation, containing a bool indicating if the update was successful.</returns>
    Task<bool> UpdateAsync(int id, string name);
    /// <summary>
    /// Deletes a platform by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the platform to delete.</param>
    /// <returns>A task that represents the asynchronous operation, containing a bool indicating if the deletion was successful.</returns>
    Task<bool> DeleteAsync(int id);
}
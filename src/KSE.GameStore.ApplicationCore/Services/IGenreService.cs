using KSE.GameStore.ApplicationCore.Models;
using KSE.GameStore.DataAccess.Entities;

namespace KSE.GameStore.ApplicationCore.Services;

/// <summary>
/// Provides methods for managing game genres.
/// </summary>
public interface IGenreService
{
    /// <summary>
    /// Retrieves a genre by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the genre.</param>
    /// <returns>The <see cref="Genre"/> with the specified ID </returns>
    /// <exception cref="BadRequestException">Thrown when the id is invalid.</exception>
    /// <exception cref="NotFoundException">Thrown when the genre does not exist.</exception>
    Task<Genre?> GetGenreByIdAsync(int id);

    /// <summary>
    /// Creates a new genre with the specified name.
    /// </summary>
    /// <param name="name">The name of the genre to create.</param>
    /// <returns>The created <see cref="Genre"/> object.</returns>
    /// <exception cref="BadRequestException">Thrown when the name is invalid or already exists.</exception>
    Task<Genre?> CreateGenreAsync(string name);

    /// <summary>
    /// Updates the name of an existing genre.
    /// </summary>
    /// <param name="id">The unique identifier of the genre to update.</param>
    /// <param name="name">The new name for the genre.</param>
    /// <returns>The updated <see cref="Genre"/> object.</returns>
    /// <exception cref="BadRequestException">Thrown when the ID or name is invalid.</exception>
    /// <exception cref="NotFoundException">Thrown when the genre does not exist.</exception>
    Task<Genre?> UpdateGenreAsync(int id, string name);

    /// <summary>
    /// Deletes a genre by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the genre to delete.</param>
    /// <returns><c>true</c> if the genre was successfully deleted; otherwise, throws an exception.</returns>
    /// <exception cref="BadRequestException">Thrown when the ID is invalid.</exception>
    /// <exception cref="NotFoundException">Thrown when the genre does not exist.</exception>
    Task<bool> DeleteGenreAsync(int id);
}
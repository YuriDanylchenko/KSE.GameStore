using KSE.GameStore.ApplicationCore.Models;
using KSE.GameStore.DataAccess.Entities;

namespace KSE.GameStore.ApplicationCore.Services;

/// <summary>
/// Provides methods for managing games and their related entities.
/// </summary>
public interface IGameService
{
    /// <summary>
    /// Retrieves a paginated list of all games.
    /// </summary>
    /// <param name="pageNumber">
    /// The 1-based page number to retrieve. If null, defaults to 1.
    /// </param>
    /// <param name="pageSize">
    /// The number of items per page. If null, defaults to 10.
    /// </param>
    /// <returns>
    /// A list of <see cref="GameDTO"/> representing the requested page of games.
    /// </returns>
    /// <exception cref="BadRequestException">
    /// Thrown when either parameter is less than or equal to zero.
    /// </exception>
    Task<List<GameDTO>> GetAllGamesAsync(int? pageNumber, int? pageSize);

    /// <summary>
    /// Retrieves a game by its unique identifier with all related entities.
    /// </summary>
    /// <param name="id">The unique identifier of the game.</param>
    /// <returns>
    /// The complete <see cref="GameDTO"/> with the specified ID including publisher,
    /// genres, platforms, current price, and region permissions.
    /// </returns>
    /// <exception cref="BadRequestException">
    /// Thrown when id is less than or equal to zero.
    /// </exception>
    /// <exception cref="NotFoundException">
    /// Thrown when no game exists with the specified id.
    /// </exception>
    Task<GameDTO> GetGameByIdAsync(int id);

    /// <summary>
    /// Creates a new game with all specified relationships.
    /// </summary>
    /// <param name="gameDto">
    /// The <see cref="GameDTO"/> containing the game details and related entity IDs.
    /// </param>
    /// <returns>
    /// The newly created <see cref="GameDTO"/> with all relationships populated.
    /// </returns>
    /// <exception cref="BadRequestException">
    /// Thrown when:
    /// - A game with the same title already exists
    /// - Required fields are missing or invalid
    /// </exception>
    /// <exception cref="NotFoundException">
    /// Thrown when referenced entities (publisher, genres, etc.) are not found.
    /// </exception>
    Task<GameDTO> CreateGameAsync(GameDTO gameDto);

    /// <summary>
    /// Updates an existing game and its relationships.
    /// Creates a new price entry while archiving the previous price.
    /// </summary>
    /// <param name="gameDto">
    /// The <see cref="GameDTO"/> containing updated game details and relationship IDs.
    /// </param>
    /// <returns>
    /// The updated <see cref="GameDTO"/> with all relationships populated.
    /// </returns>
    /// <exception cref="BadRequestException">
    /// Thrown when:
    /// - The new title conflicts with another game
    /// - Required fields are missing or invalid
    /// </exception>
    /// <exception cref="NotFoundException">
    /// Thrown when:
    /// - The game to update doesn't exist
    /// - Referenced entities (publisher, genres, etc.) are not found
    /// </exception>
    Task<GameDTO> UpdateGameAsync(GameDTO gameDto);

    /// <summary>
    /// Deletes a game by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the game to delete.</param>
    /// <returns>
    /// A task representing the asynchronous delete operation.
    /// </returns>
    /// <exception cref="BadRequestException">
    /// Thrown when id is less than or equal to zero.
    /// </exception>
    /// <exception cref="NotFoundException">
    /// Thrown when no game exists with the specified id.
    /// </exception>
    Task DeleteGameAsync(int id);

    /// <summary>
    /// Retrieves all games belonging to a specific genre.
    /// </summary>
    /// <param name="genreId">The unique identifier of the genre.</param>
    /// <returns>
    /// A list of <see cref="GameDTO"/> objects for games in the specified genre.
    /// </returns>
    /// <exception cref="NotFoundException">
    /// Thrown when the specified genre doesn't exist.
    /// </exception>
    Task<List<GameDTO>> GetGamesByGenreAsync(int genreId);

    /// <summary>
    /// Retrieves all games available on a specific platform.
    /// </summary>
    /// <param name="platformId">The unique identifier of the platform.</param>
    /// <returns>
    /// A list of <see cref="GameDTO"/> objects for games on the specified platform.
    /// </returns>
    /// <exception cref="NotFoundException">
    /// Thrown when the specified platform doesn't exist.
    /// </exception>
    Task<List<GameDTO>> GetGamesByPlatformAsync(int platformId);
}
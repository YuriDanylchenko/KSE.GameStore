using KSE.GameStore.ApplicationCore.Models;
using KSE.GameStore.ApplicationCore.Requests.Games;

namespace KSE.GameStore.ApplicationCore.Interfaces;

/// <summary>
/// Provides methods for managing games.
/// </summary>
public interface IGameService
{
    /// <summary>
    /// Retrieves a paginated list of all games.
    /// </summary>
    /// <param name="pageNumber">
    /// The 1-based page number to retrieve. If <see langword="null"/>, defaults to <c>1</c>.
    /// </param>
    /// <param name="pageSize">
    /// The number of items per page. If <see langword="null"/>, defaults to <c>10</c>.
    /// </param>
    /// <returns>
    /// A list of <see cref="GameDTO"/> representing the requested page of games.
    /// </returns>
    Task<List<GameDTO>> GetAllGamesAsync(int? pageNumber, int? pageSize);

    /// <summary>
    /// Retrieves a game by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the game.</param>
    /// <returns>
    /// The <see cref="GameDTO"/> with the specified ID.
    /// </returns>
    /// <exception cref="BadRequestException">
    /// Thrown when <paramref name="id"/> is less than or equal to zero.
    /// </exception>
    /// <exception cref="NotFoundException">
    /// Thrown when no game exists with the specified <paramref name="id"/>.
    /// </exception>
    Task<GameDTO> GetGameByIdAsync(int id);

    /// <summary>
    /// Creates a new game using the provided data.
    /// </summary>
    /// <param name="createGameRequest">
    /// A <see cref="CreateGameRequest"/> containing the details of the game to create.
    /// </param>
    /// <returns>
    /// The newly created <see cref="GameDTO"/>, including its assigned ID.
    /// </returns>
    /// <exception cref="BadRequestException">
    /// Thrown when the request data is invalid or a game with the same title already exists.
    /// </exception>
    Task<GameDTO> CreateGameAsync(CreateGameRequest createGameRequest);

    /// <summary>
    /// Updates an existing game with the provided data.
    /// </summary>
    /// <param name="updateGameRequest">
    /// An <see cref="UpdateGameRequest"/> containing the ID of the game to update and the new values.
    /// </param>
    /// <returns>
    /// The updated <see cref="GameDTO"/> reflecting the applied changes.
    /// </returns>
    /// <exception cref="BadRequestException">
    /// Thrown when the request data is invalid or the updated title conflicts with another game.
    /// </exception>
    /// <exception cref="NotFoundException">
    /// Thrown when no game exists with the ID specified in <paramref name="updateGameRequest"/>.
    /// </exception>
    Task<GameDTO> UpdateGameAsync(UpdateGameRequest updateGameRequest);

    /// <summary>
    /// Deletes a game by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the game to delete.</param>
    /// <returns>
    /// A task representing the asynchronous delete operation.
    /// </returns>
    /// <exception cref="BadRequestException">
    /// Thrown when the <paramref name="id"/> is less than or equal to zero.
    /// </exception>
    /// <exception cref="NotFoundException">
    /// Thrown when no game exists with the specified <paramref name="id"/>.
    /// </exception>
    Task DeleteGameAsync(int id);
}
using GameShelf.API.Backend.Models;
using System.ComponentModel.DataAnnotations;

namespace GameShelf.API.DTOs
{
    public class LibraryDtos
    {
        public record AddGameToLibraryDto(
    [Required] int RawgGameId,
    [Required] GameStatus Status,
    string? Platform // Platform è specifica dell'utente
);

        public record UpdateGameDto(
            [Required] GameStatus Status,
            [Range(1, 5)] int? UserRating
        );

    }
}

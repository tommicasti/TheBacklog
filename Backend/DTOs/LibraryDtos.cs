using GameShelf.API.Backend.Models;
using System.ComponentModel.DataAnnotations;

namespace GameShelf.API.DTOs
{
    public class LibraryDtos
    {
        public record AddGameDto(
            [Required] int RawgGameId,
            [Required] string GameTitle,
            string CoverImageUrl,
            [Required] GameStatus Status
        );

        public record UpdateGameDto(
            [Required] GameStatus Status
        );
    }
}

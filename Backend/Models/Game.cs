namespace GameShelf.API.Models
{
    public class Game
    {
        public int Id { get; set; } // Chiave primaria interna
        public int RawgGameId { get; set; } // ID univoco da RAWG
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? BackgroundImageUrl { get; set; }
        public int? MetacriticScore { get; set; }
        public string? Released { get; set; }

    }
}

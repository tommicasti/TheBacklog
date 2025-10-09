namespace GameShelf.API.DTOs
{
    public class GameDtos
    {
        public class GameSummaryDto
        {
            public int Id { get; set; } // RawgGameId
            public string Name { get; set; } = string.Empty;
            public string BackgroundImageUrl { get; set; } = string.Empty;
            public int? MetacriticScore { get; set; }
            public string Released { get; set; } = string.Empty;
        }

        // DTO per la pagina di dettaglio (più completo)
        public class GameDetailDto
        {
            public int Id { get; set; } // RawgGameId
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string BackgroundImageUrl { get; set; } = string.Empty;
            public int? MetacriticScore { get; set; }
            public string Released { get; set; } = string.Empty;
            public List<string> Platforms { get; set; } = new();
            public List<string> Genres { get; set; } = new();
        }
    }
}

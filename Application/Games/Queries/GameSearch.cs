using Application.DTO;
using Application.Games.Services;
using craftersmine.SteamGridDBNet;
using MediatR;

namespace Application.Games.Queries;

public class GameSearchRequest: IRequest<List<GameSearchListingDto>>
{
    public string Query { get; set; } = null!;
}

public class GameSearchRequestHandler : IRequestHandler<GameSearchRequest, List<GameSearchListingDto>>
{
    private readonly GameCacheService _gameCacheService;

    public GameSearchRequestHandler(GameCacheService gameCacheService)
    {
        _gameCacheService = gameCacheService;
    }

    public async Task<List<GameSearchListingDto>> Handle(GameSearchRequest request, CancellationToken cancellationToken)
    {
        var steamGrid = new SteamGridDb("d4797e6e1502c29aace2e94aed09f51f");

        var cache = await _gameCacheService.TryHitCacheAsync(request.Query);
        
        if (cache != null)
        {
            return cache.Results;
        }
        
        var search = await steamGrid.SearchForGamesAsync(request.Query);
        
        var results = new List<GameSearchListingDto>();
        
        foreach (var steamGridDbGame in search)
        {
            var gameIcon = await steamGrid.GetIconsForGameAsync(steamGridDbGame, styles: SteamGridDbStyles.AllIcons,
                limit: 1);
            
            var game = new GameSearchListingDto
            {
                Name = steamGridDbGame.Name,
                IconUrl = gameIcon.FirstOrDefault()?.ThumbnailImageUrl ?? string.Empty,
                GameId = steamGridDbGame.Id.ToString()
            };
            
            results.Add(game);
        }

        await _gameCacheService.TryAddCacheAsync(request.Query, results);

        return results;
    }
}
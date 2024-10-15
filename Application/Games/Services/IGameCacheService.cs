using Application.DTO;

namespace Application.Games.Services;

public interface IGameCacheService
{
    public GameSearchCacheResult? TryHitCacheAsync(string query);
    public void TryAddCacheAsync(string query, List<GameSearchListingDto> results);
}
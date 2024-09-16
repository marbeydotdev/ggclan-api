using Application.DTO;

namespace Application.Games.Services;

public class GameSearchCacheResult
{
    public string Query { get; set; } = null!;
    public List<GameSearchListingDto> Results { get; set; } = null!;
}

public class GameCacheService
{
    private List<GameSearchCacheResult> _results = [];

    public async Task<GameSearchCacheResult?> TryHitCacheAsync(string query)
    {
        lock (_results)
        {
            var result = _results.FirstOrDefault(x => x.Query == query.ToLower());
            return result;
        }
    }

    public async Task TryAddCacheAsync(string query, List<GameSearchListingDto> results)
    {
        if (await TryHitCacheAsync(query) != null)
        {
            return;
        }

        lock (_results)
        {
            _results.Add(new ()
            {
                Query = query.ToLower(),
                Results = results
            });
        }
    }
}
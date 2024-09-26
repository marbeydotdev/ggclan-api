using System.Security.Claims;

namespace WebAPI;

public static class Extensions
{
    public static string GetNameIdentifier(this HttpContext context)
    {
        return context.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
    }
}
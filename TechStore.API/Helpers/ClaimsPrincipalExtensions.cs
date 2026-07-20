using System.Security.Claims;

namespace TechStore.API.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static int? GetUserId(this ClaimsPrincipal user)
        {
            string? value = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(value, out int userId) ? userId : null;
        }
    }
}

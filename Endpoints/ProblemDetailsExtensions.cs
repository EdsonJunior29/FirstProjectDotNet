using Flunt.Notifications;
using Microsoft.AspNetCore.Identity;
using System.Runtime.CompilerServices;

namespace FirstProjectDotNetCore.Endpoints;

public static class ProblemDetailsExtensions
{
    public static Dictionary<string, string[]> ConvertToProblemDetails(this IReadOnlyCollection<Notification> notifications)
    { 
        return notifications
               .GroupBy(c => c.Key)
               .ToDictionary(c => c.Key, c => c.Select(x => x.Message).ToArray());
    }

    public static Dictionary<string, string[]> ConvertToProblemDetails(this IEnumerable<IdentityError> errors)
    {
        return errors
               .GroupBy(c => c.Code)
               .ToDictionary(c => c.Key, c => c.Select(x => x.Description).ToArray());
    }
}

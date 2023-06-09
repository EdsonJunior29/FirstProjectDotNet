using FirstProjectDotNetCore.Infra.Services;
using Microsoft.AspNetCore.Authorization;

namespace FirstProjectDotNetCore.Endpoints.Categories;

public static class UserGetAll
{
    public static string Template => "/users";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "UserPolicy")]
    public static async Task<IResult> Action(int? page, int? rows, QueryAllUsersWithClaimsName query)
    {
        if (page == null) { page = 1; }

        if (rows == null) { rows = 2; }

        var result = await query.Execute(page.Value, rows.Value);
        return Results.Ok(result);
    }
}

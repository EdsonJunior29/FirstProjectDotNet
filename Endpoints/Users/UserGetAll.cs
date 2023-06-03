using FirstProjectDotNetCore.Infra.Services;

namespace FirstProjectDotNetCore.Endpoints.Categories;

public static class UserGetAll
{
    public static string Template => "/users";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    public static IResult Action(int? page, int? rows, QueryAllUsersWithClaimsName query)
    {
        if (page == null) { page = 1; }

        if (rows == null) { rows = 2; }

        return Results.Ok(query.execute(page.Value, rows.Value));
    }
}

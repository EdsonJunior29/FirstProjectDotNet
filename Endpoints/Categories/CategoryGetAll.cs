using FirstProjectDotNetCore.Infra.Services;
using Microsoft.AspNetCore.Authorization;

namespace FirstProjectDotNetCore.Endpoints.Categories;

public static class CategoryGetAll
{
    public static string Template => "/categories";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "UserPolicy")]
    public static async Task<IResult> Action(int? page, int? rows, QueryAllCategories allCategories) {


        if (page == null) { page = 1; }

        if (rows == null) { rows = 2; }

        var categories = await allCategories.Execute(page.Value, rows.Value);

        return Results.Ok(categories);
    }

}

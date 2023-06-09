using FirstProjectDotNetCore.Infra.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace FirstProjectDotNetCore.Endpoints.Categories;

public static class CategoryGetAll
{
    public static string Template => "/categories";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "UserPolicy")]
    public static async Task<IResult> Action(ApplicationDbContext context) {

        var categories = await context.Categories.ToListAsync();
        var response = categories.Select(c => new CategoryResponse
        {
            Id = c.Id,
            Name = c.Name,
            Active = c.Active
        });

        return Results.Ok(response);
    }

}

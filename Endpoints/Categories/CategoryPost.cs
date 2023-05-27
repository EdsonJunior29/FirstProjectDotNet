using FirstProjectDotNetCore.Domain.Products;
using FirstProjectDotNetCore.Infra.Data;

namespace FirstProjectDotNetCore.Endpoints.Categories;

public static class CategoryPost
{
    public static string Template => "/categories";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    public static IResult Action(CategoryDto categoryDto, ApplicationDbContext context) {
        var category = new Category(categoryDto.Name, "Edson Junior", "Edson Junior");

        if (!category.IsValid) {
            return Results.ValidationProblem(category.Notifications.ConvertToProblemDetails());
        }
        context.Categories.Add(category);
        context.SaveChanges();

        return Results.Created("/categories", category.Id);
    }

}

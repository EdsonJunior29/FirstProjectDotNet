using FirstProjectDotNetCore.Domain.Products;
using FirstProjectDotNetCore.Infra.Data;
using Microsoft.AspNetCore.Mvc;

namespace FirstProjectDotNetCore.Endpoints.Categories;

public static class CategoryPut
{
    public static string Template => "/categories/{id:guid}";
    public static string[] Methods => new string[] { HttpMethod.Put.ToString() };
    public static Delegate Handle => Action;

    public static IResult Action([FromRoute] Guid Id, CategoryDto categoryDto, ApplicationDbContext context) {
        var category = context.Categories.Where(c => c.Id == Id).FirstOrDefault();
        
        if (category == null)
        {
            return Results.NotFound();
        }

        category.EditCategory(categoryDto.Name, categoryDto.Active);

        if(!category.IsValid)
        {
            return Results.ValidationProblem(category.Notifications.ConvertToProblemDetails());
        }
     
        context.SaveChanges();

        return Results.NoContent();
    }

}

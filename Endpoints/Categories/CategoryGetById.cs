using FirstProjectDotNetCore.Domain.Products;
using FirstProjectDotNetCore.Infra.Data;
using Microsoft.AspNetCore.Mvc;

namespace FirstProjectDotNetCore.Endpoints.Categories;

public static class CategoryGetById
{
    public static string Template => "/categories/{id:guid}";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    public static IResult Action([FromRoute] Guid Id, ApplicationDbContext context) {

        var categories = context.Categories.FirstOrDefault(c => c.Id == Id);
        var response = new CategoryDto
        {
            Name = categories.Name,
            Active = categories.Active,
        };

        return Results.Ok(response);
    }

}

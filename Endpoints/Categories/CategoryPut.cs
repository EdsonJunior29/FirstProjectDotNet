using FirstProjectDotNetCore.Domain.Products;
using FirstProjectDotNetCore.Infra.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;
using System.Security.Claims;

namespace FirstProjectDotNetCore.Endpoints.Categories;

public static class CategoryPut
{
    public static string Template => "/categories/{id:guid}";
    public static string[] Methods => new string[] { HttpMethod.Put.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "UserPolicy02")]
    public static IResult Action([FromRoute] Guid Id, CategoryDto categoryDto, HttpContext http, ApplicationDbContext context) {

        //obter informações do usuário que está autenticado
        var userId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var category = context.Categories.Where(c => c.Id == Id).FirstOrDefault();
        
        if (category == null)
        {
            return Results.NotFound();
        }

        category.EditCategory(categoryDto.Name, categoryDto.Active, userId);

        if(!category.IsValid)
        {
            return Results.ValidationProblem(category.Notifications.ConvertToProblemDetails());
        }
     
        context.SaveChanges();

        return Results.NoContent();
    }

}

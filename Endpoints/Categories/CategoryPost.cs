using FirstProjectDotNetCore.Domain.Products;
using FirstProjectDotNetCore.Infra.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FirstProjectDotNetCore.Endpoints.Categories;

public static class CategoryPost
{
    public static string Template => "/categories";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "UserPolicy02")]
    public static IResult Action(CategoryDto categoryDto, HttpContext http, ApplicationDbContext context) {
        //obter informações do usuário que está autenticado
        var userId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        
        var category = new Category(categoryDto.Name, userId, userId);

        if (!category.IsValid) {
            return Results.ValidationProblem(category.Notifications.ConvertToProblemDetails());
        }
        context.Categories.Add(category);
        context.SaveChanges();

        return Results.Created("/categories", category.Id);
    }

    /* Tipo de proteções
     
        [AllowAnonymous] => qualquer usuário poderá ter acesso ao esse método.
        [Authorize] => somente usuários autenticados terá acesso a esse método.

     */

}

using FirstProjectDotNetCore.Domain.Products;
using FirstProjectDotNetCore.Endpoints.Users;
using FirstProjectDotNetCore.Infra.Data;
using Microsoft.AspNetCore.Identity;

namespace FirstProjectDotNetCore.Endpoints.Categories;

public static class UserPost
{
    public static string Template => "/users";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    public static IResult Action(UserDto userDto, UserManager<IdentityUser> userManager)
    {
        var user = new IdentityUser { UserName = userDto.Email, Email = userDto.Email };
        var result = userManager.CreateAsync(user, userDto.Password).Result;

        if (!result.Succeeded){ 
            return Results.BadRequest(result.Errors.First());
        }

        return Results.Created("/users", user.Id);
    }

}

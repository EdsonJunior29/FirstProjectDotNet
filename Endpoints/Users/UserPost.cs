using FirstProjectDotNetCore.Endpoints.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace FirstProjectDotNetCore.Endpoints.Categories;

public static class UserPost
{
    public static string Template => "/users";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    [Authorize]
    public static IResult Action(UserDto userDto, UserManager<IdentityUser> userManager)
    {
        var user = new IdentityUser { UserName = userDto.Email, Email = userDto.Email };
        var result = userManager.CreateAsync(user, userDto.Password).Result;

        if (!verifySuccess(result))
        { 
            return Results.ValidationProblem(result.Errors.ConvertToProblemDetails());
        }

        var userClaims = new List<Claim>
        {
            new Claim("UserCode", userDto.UserCode),
            new Claim("Name", userDto.Name)
        };

        var claimResult = userManager.AddClaimsAsync(user, userClaims).Result;

        if (!verifySuccess(claimResult))
        {
            return Results.BadRequest(claimResult.Errors.First());
        }

        return Results.Created("/users", user.Id);
    }

    private static bool verifySuccess(IdentityResult identity) 
    {
        if (identity.Succeeded) {
            return true;
        }

        return false;
    }

}

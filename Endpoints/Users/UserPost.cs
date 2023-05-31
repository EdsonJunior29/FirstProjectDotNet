using FirstProjectDotNetCore.Endpoints.Users;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

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

        if (!verifySuccess(result))
        { 
            return Results.BadRequest(result.Errors.First());
        }

        var claimResult = userManager.AddClaimAsync(user, new Claim("UserCode", userDto.UserCode)).Result;

        if (!verifySuccess(claimResult)) {
            return Results.BadRequest(claimResult.Errors.First());
        }

        claimResult = userManager.AddClaimAsync(user, new Claim("Name", userDto.Name)).Result;

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

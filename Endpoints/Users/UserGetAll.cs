using FirstProjectDotNetCore.Endpoints.Users;
using Microsoft.AspNetCore.Identity;

namespace FirstProjectDotNetCore.Endpoints.Categories;

public static class UserGetAll
{
    public static string Template => "/users";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    public static IResult Action(UserManager<IdentityUser> userManager)
    {
        var users = userManager.Users.ToList();

        /*buscar usuários e seu claims(Solução perigosa)
         * Motivo: A cada interação o sistema consulta o banco de dados.
            (Afetando a performance) 
            Para resolver a performance temos que criar paginação
         */
        var employees = new List<UserResponse>();

        foreach (var user in users)
        {
            var claims = userManager.GetClaimsAsync(user).Result;
            var claimName = claims.FirstOrDefault(c => c.Type == "Name");
            var userName = claimName != null ? claimName.Value : string.Empty;

            employees.Add(new UserResponse(userName, user.Email));
        
        }
        return Results.Ok(employees);
    }
}

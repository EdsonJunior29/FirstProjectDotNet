using Dapper;
using FirstProjectDotNetCore.Endpoints.Users;
using Microsoft.Data.SqlClient;

namespace FirstProjectDotNetCore.Endpoints.Categories;

public static class UserGetAll
{
    public static string Template => "/users";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    public static IResult Action(int? page, IConfiguration Configuration)
    {
        int rows = 2;

        if (page == null) { 
            page = 1;
        }

        //criando uma nova conexão com o banco de dados SQL
        var db = new SqlConnection(Configuration["ConnectionStrings:FirstProjectDotNet"]);

        var query = @"SELECT
                        AspNetUsers.Email,
                        AspNetUserClaims.ClaimValue as Name
                        FROM AspNetUsers
                        INNER JOIN AspNetUserClaims
                            on AspNetUsers.Id = AspNetUserClaims.UserId and ClaimType = 'Name'
                        Order By Name
                        OFFSET (@page - 1) * @rows ROWS FETCH NEXT @rows ROWS ONLY"; // Linha responsável pela paginação.

        var employees = db.Query<UserResponse>(
               query,
               new { page, rows}
            );

        return Results.Ok(employees);
    }
}

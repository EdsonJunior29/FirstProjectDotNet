using Dapper;
using FirstProjectDotNetCore.Endpoints.Users;
using Microsoft.Data.SqlClient;

namespace FirstProjectDotNetCore.Infra.Services;

public class QueryAllUsersWithClaimsName
{
    private readonly IConfiguration configuration;

    //Ijetando Iconfiguration nessa classe
    public QueryAllUsersWithClaimsName(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public IEnumerable<UserResponse> execute(int page, int rows) 
    {
        //criando uma nova conexão com o banco de dados SQL
        var db = new SqlConnection(configuration["ConnectionStrings:FirstProjectDotNet"]);

        var query = @"SELECT
                        AspNetUsers.Email,
                        AspNetUserClaims.ClaimValue as Name
                        FROM AspNetUsers
                        INNER JOIN AspNetUserClaims
                            on AspNetUsers.Id = AspNetUserClaims.UserId and ClaimType = 'Name'
                        Order By Name
                        OFFSET (@page - 1) * @rows ROWS FETCH NEXT @rows ROWS ONLY"; // Linha responsável pela paginação.

         return db.Query<UserResponse>(
               query,
               new { page, rows }
            );

    }
}

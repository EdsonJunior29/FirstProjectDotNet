using Dapper;
using FirstProjectDotNetCore.Endpoints.Categories;
using Microsoft.Data.SqlClient;

namespace FirstProjectDotNetCore.Infra.Services;

public class QueryAllCategories
{
    private readonly IConfiguration configuration;
    public QueryAllCategories(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public async Task<IEnumerable<CategoryResponse>> Execute(int page, int rows)
    {
        //criando uma nova conexão com o banco de dados SQL
        var db = new SqlConnection(configuration["ConnectionStrings:FirstProjectDotNet"]);

        var query = @"SELECT
                       Categories.Id,
                       Categories.Name,
                       Categories.Active
                    FROM Categories
                    WHERE Categories.Active = 1
                    Order By Name
                        OFFSET (@page - 1) * @rows ROWS FETCH NEXT @rows ROWS ONLY";

        return await db.QueryAsync<CategoryResponse>(
              query,
              new { page, rows }
           );
    }
}

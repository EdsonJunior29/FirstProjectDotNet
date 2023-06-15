using FirstProjectDotNetCore.Infra.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace FirstProjectDotNetCore.Endpoints.Products;

public class ProductGetShowCase
{
      public static string Template => "/products/showcase";
      public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
      public static Delegate Handle => Action;

    [AllowAnonymous]
    public static async Task<IResult> Action(int? page, int? row, string? orderBy, ApplicationDbContext context)
     {
          if (page == null) page = 1;
          if (row == null) row = 1;
          if (string.IsNullOrEmpty(orderBy)) orderBy = "Name";

          var queryBase = context.Products.Include(p => p.Category)
              .Where(p => p.HasStock && p.Category.Active);

          var queryFilter = queryBase.Skip((page.Value - 1) * row.Value).Take(row.Value);
          if (orderBy == "Name")
          {
              queryFilter = queryFilter.OrderBy(p => p.Name);
          }
          else {
              queryFilter = queryFilter.OrderBy(p => p.Price);
          }

          var products = queryFilter.ToList();

          var response = products.Select(p => new ProductResponse(p.Name, p.Category.Name, p.Description,p.Price, p.HasStock, p.Active));

          return Results.Ok(response);
      }
}

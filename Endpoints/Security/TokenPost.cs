using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FirstProjectDotNetCore.Endpoints.Security;

public static class TokenPost
{
    public static string Template => "/token";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    [AllowAnonymous]
    public static IResult Action(Login login, UserManager<IdentityUser> userManager, IConfiguration configuration) 
    {
        //procurar o usuário no banco de dados
        var user = userManager.FindByEmailAsync(login.Email).Result;

        if (!userManager.CheckPasswordAsync(user, login.Password).Result || user == null) 
        {
            Results.BadRequest("Usuário ou senha incorreto.");
        }

        //Criar um token (Código padrão do identity .NET)
        var key = Encoding.ASCII.GetBytes(configuration["JwtBearerTokenSetting:SecretKey"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            //Informações necessário para criar o token
            Subject = new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.Email, login.Email),
                new Claim("UserCode", "1"),
            }),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Audience = configuration["JwtBearerTokenSetting:Audience"],
            Issuer = configuration["JwtBearerTokenSetting:Issuer"]
        };

        //Gerando o token
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Results.Ok(new
        {
            token = tokenHandler.WriteToken(token)
        });

    }
}

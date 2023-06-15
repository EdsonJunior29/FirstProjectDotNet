using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FirstProjectDotNetCore.Endpoints.Security;

public class TokenPost
{
    public static string Template => "/token";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    [AllowAnonymous]
    public static async Task<IResult> Action(
        Login login,
        UserManager<IdentityUser> userManager,
        IConfiguration configuration,
        ILogger<TokenPost> logger,
        IWebHostEnvironment environment
        )
    // IWebHostEnvironment : informa em que ambiente o código está sendo executado ex:. Dev e Produção
    {
        DateTime dateTime = DateTime.UtcNow;
        //adicionar um log
        logger.LogInformation("Getting token" + dateTime);
        
        //Log de Warning
        //logger.LogWarning("Warning Getting token" + dateTime);
        

        //procurar o usuário no banco de dados
        var user = await userManager.FindByEmailAsync(login.Email);

        if (!await userManager.CheckPasswordAsync(user, login.Password) || user == null) 
        {
            //Log de erro
            logger.LogError("Erro Getting User" + dateTime);
            Results.BadRequest("Usuário ou senha incorreto.");
        }

        var claims = await userManager.GetClaimsAsync(user);

        var subject = new ClaimsIdentity(new Claim[] {
            new Claim(ClaimTypes.Email, login.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
        });

        subject.AddClaims(claims);

        //Criar um token (Código padrão do identity .NET)
        var key = Encoding.ASCII.GetBytes(configuration["JwtBearerTokenSetting:SecretKey"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            //Informações necessário para criar o token
            Subject = subject,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Audience = configuration["JwtBearerTokenSetting:Audience"],
            Issuer = configuration["JwtBearerTokenSetting:Issuer"],
            Expires = environment.IsDevelopment() || environment.IsStaging() ? DateTime.UtcNow.AddMonths(1) : DateTime.UtcNow.AddMinutes(2) 
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

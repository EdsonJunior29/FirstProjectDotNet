
using FirstProjectDotNetCore.Endpoints.Categories;
using FirstProjectDotNetCore.Endpoints.Security;
using FirstProjectDotNetCore.Infra.Data;
using FirstProjectDotNetCore.Infra.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FirstProjectDotNetCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Configuration Conection DB
            builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["ConnectionStrings:FirstProjectDotNet"]);

            //Configuration Identity
            builder.Services.AddIdentity<IdentityUser, IdentityRole>( options => {
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>();

            // Add services to the container.(Autorização)
            builder.Services.AddAuthorization(options => {
                /* Com esse código, todas requisições deverão ser feitas
                    por usuários autenticados.
                 */
                /*options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();*/

                //Criando uma policies
                options.AddPolicy("UserPolicy", up =>
                {
                    up.RequireAuthenticatedUser().RequireClaim("UserCode");
                });
            });

            //Habilitando o serviço de autenticação
            builder.Services.AddAuthentication(x => {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateActor = true, 
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JwtBearerTokenSetting:Issuer"],
                    ValidAudience = builder.Configuration["JwtBearerTokenSetting:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtBearerTokenSetting:SecretKey"]))
                };
            
            });

            //Adicionando a classe QueryAllUsersWithClaimsName como um serviço
            builder.Services.AddScoped<QueryAllUsersWithClaimsName>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseAuthentication();
            app.UseAuthorization();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            //Methods for requests
            app.MapMethods(CategoryPost.Template, CategoryPost.Methods, CategoryPost.Handle);
            app.MapMethods(CategoryGetAll.Template, CategoryGetAll.Methods, CategoryGetAll.Handle);
            app.MapMethods(CategoryPut.Template, CategoryPut.Methods, CategoryPut.Handle);
            app.MapMethods(CategoryGetById.Template, CategoryGetById.Methods, CategoryGetById.Handle);

            //Methods Users
            app.MapMethods(UserPost.Template, UserPost.Methods, UserPost.Handle);
            app.MapMethods(UserGetAll.Template, UserGetAll.Methods, UserGetAll.Handle);

            //Methods para gerar o token
            app.MapMethods(TokenPost.Template, TokenPost.Methods, TokenPost.Handle);

            app.Run();
        }
    }
}
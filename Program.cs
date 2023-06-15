
using FirstProjectDotNetCore.Endpoints.Categories;
using FirstProjectDotNetCore.Endpoints.Products;
using FirstProjectDotNetCore.Endpoints.Security;
using FirstProjectDotNetCore.Infra.Data;
using FirstProjectDotNetCore.Infra.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

namespace FirstProjectDotNetCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Configuração do Serilog(Salvar informações de logs)
            builder.WebHost.UseSerilog((context, configuration) =>
            {
                configuration
                    .WriteTo.Console()
                    .WriteTo.MSSqlServer(
                        context.Configuration["ConnectionStrings:FirstProjectDotNet"],
                        sinkOptions: new Serilog.Sinks.MSSqlServer.MSSqlServerSinkOptions()
                        {
                            AutoCreateSqlTable = true,
                            TableName = "LogAPI"
                        }
                    );
            });

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

                //Informando o usuario que terá acesse a rota.
                //Essa policie valida o claim e o valor do claim
                options.AddPolicy("UserPolicy02", up =>
                {
                    up.RequireAuthenticatedUser().RequireClaim("UserCode", "usuario02");
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
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = builder.Configuration["JwtBearerTokenSetting:Issuer"],
                    ValidAudience = builder.Configuration["JwtBearerTokenSetting:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtBearerTokenSetting:SecretKey"]))
                };
            
            });

            //Adicionando a classe QueryAllUsersWithClaimsName and QueryAllCategories como um serviço
            builder.Services.AddScoped<QueryAllUsersWithClaimsName>();
            builder.Services.AddScoped<QueryAllCategories>();

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

            //Methods Products
            app.MapMethods(ProductPost.Template, ProductPost.Methods, ProductPost.Handle);
            app.MapMethods(ProductGetAll.Template, ProductGetAll.Methods, ProductGetAll.Handle);

            //Methods Vitrine de Produtos
            app.MapMethods(ProductGetShowCase.Template, ProductGetShowCase.Methods, ProductGetShowCase.Handle);

            //Methods para gerar o token
            app.MapMethods(TokenPost.Template, TokenPost.Methods, TokenPost.Handle);

            //Filtros de erros
            app.UseExceptionHandler("/error");

            //Tratamento de erro
            app.Map("/error", (HttpContext http) =>
            {
                //verificar o tipo de erro(Interrogação e para saver se existe ou não o erro);
                var error = http.Features?.Get<IExceptionHandlerFeature>()?.Error;

                if (error != null) {
                    if (error is SqlException) {
                        return Results.Problem(title: "Database out", statusCode: 500);
                        
                    }
                    
                }

                return Results.Problem(title: "An error ocurred", statusCode: 500);

            });

            app.Run();
        }
    }
}

using FirstProjectDotNetCore.Endpoints.Categories;
using FirstProjectDotNetCore.Infra.Data;
using FirstProjectDotNetCore.Infra.Services;
using Microsoft.AspNetCore.Identity;

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

            // Add services to the container.
            builder.Services.AddAuthorization();

            //Adicionando a classe QueryAllUsersWithClaimsName como um serviço
            builder.Services.AddScoped<QueryAllUsersWithClaimsName>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

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


            app.Run();
        }
    }
}
﻿using FirstProjectDotNetCore.Domain.Products;
using Flunt.Notifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FirstProjectDotNetCore.Infra.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options) { }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Ignore<Notification>();

        modelBuilder.Entity<Category>()
           .Property(c => c.Name).IsRequired();


        modelBuilder.Entity<Product>()
            .Property(p => p.Name).IsRequired();
        modelBuilder.Entity<Product>()
            .Property(p => p.Description).HasMaxLength(255);
        modelBuilder.Entity<Product>()
            .Property(p => p.CategoryId)
            .IsRequired();
        modelBuilder.Entity<Product>()
            .Property(p => p.Price).HasColumnType("decimal(10,2)").IsRequired();
    }

    //Conversão padrão Global
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        //Explicação: Todos os campos de string relacionados as classes(Category e Product por exemplo) 
        //Serão salvos no banco com o valor máximo de 100 caracteres.
        configurationBuilder.Properties<string>()
            .HaveMaxLength(100);

    }
}

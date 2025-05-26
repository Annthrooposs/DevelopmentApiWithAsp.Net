using Microsoft.EntityFrameworkCore;
using MinimalAPIProfesional.Data.Models;



namespace MinimalAPIProfesional.Data
{
     public class ApiDbContext : DbContext
     {
          public DbSet<Person> Persons { get; set; }





          // Constructeurs --------------------------------------------------------------------------------------------------------------------------
          public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
          {
          }





          // Création du modèle ---------------------------------------------------------------------------------------------------------------------
          protected override void OnModelCreating(ModelBuilder modelBuilder)
          {
               modelBuilder.Entity<Person>(c =>
               {
                    c.ToTable("Personnes");
                    c.Property(p => p.FirstName).HasMaxLength(256);
                    c.Property(p => p.LastName).HasMaxLength(256);
                    //c.Ignore(p => p.Birthday);
               });
               //base.OnModelCreating(modelBuilder);
          }




          //// Configuration de l'accès aux bases de données ------------------------------------------------------------------------------------------
          //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
          //{
          //     // SqlLite
          //     //optionsBuilder.UseSqlite("Filename=api.db");

          //     // Sql Server
          //     optionsBuilder.UseSqlServer("Data Source = localhost; Initial Catalog = api_d; Trusted_Connection = True; Trust Server Certificate = true; Integrated Security = true; MultipleActiveResultSets = true");

          //     base.OnConfiguring(optionsBuilder);
          //}
     }
}

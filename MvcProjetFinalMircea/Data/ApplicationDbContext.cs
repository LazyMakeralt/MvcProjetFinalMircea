using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MvcProjetFinalMircea.Models;

namespace MvcProjetFinalMircea.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Article> Articles { get; set; } = default!;
        public DbSet<ArticleLike> ArticleLikes { get; set; } = default!;
        public DbSet<PortfolioItem> PortfolioItems { get; set; } = default!;

        public DbSet<Commentaire> Commentaires { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Commentaire>()
                .HasOne(c => c.Auteur)
                .WithMany()
                .HasForeignKey(c => c.AuteurId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ArticleLike>()
                .HasOne(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ArticleLike>()
                .HasIndex(l => new { l.ArticleId, l.UserId })
                .IsUnique();

            builder.Entity<Commentaire>()
                .HasOne(c => c.Article)
                .WithMany(a => a.Commentaires)
                .HasForeignKey(c => c.ArticleId)
                .OnDelete(DeleteBehavior.Cascade); 

            builder.Entity<Commentaire>()
                .HasOne(c => c.Auteur)
                .WithMany()
                .HasForeignKey(c => c.AuteurId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}

using ApplicationRequestIt.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApplicationRequestIt.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);


            //builder.Entity<AanvraagBehandelaar>()
            //    .HasKey(t => new { t.AanvraagId, t.BehandelaarId });

            //builder.Entity<AanvraagBehandelaar>()
            //    .HasOne(ab => ab.Aanvraag)
            //    .WithMany(p => p.AanvraagBehandelaars)
            //    .HasForeignKey(ab => ab.AanvraagId);

            //builder.Entity<AanvraagBehandelaar>()
            //   .HasOne(ab => ab.Behandelaar)
            //   .WithMany(p => p.AanvraagBehandelaars)
            //   .HasForeignKey(ab => ab.BehandelaarId);


            builder.Entity<Aanvraag>(entity =>
           {
               entity.HasOne(d => d.ApplicationUser)
              .WithMany(a => a.CustomerAanvragen)
              .HasForeignKey(d => d.UserId)
              .HasConstraintName("FK_Aanvraag_ApplicationUser_customer");

               entity.HasOne(d => d.BehandelaarApplicationUser)
               .WithMany(a => a.BehandelaarAanvragen)
               .HasForeignKey(d => d.BehandelaarId)
               .HasConstraintName("FK_Aanvraag_ApplicationUser_Behandelaar");
           });
        }

        public DbSet<Aanvraag> Aanvragen { get; set; }
        public DbSet<Bericht> Berichten { get; set; }
        public DbSet<Status> Statussen { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }

    }
}

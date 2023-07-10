using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TestMVC.Models
{
    public class ClientsContext : DbContext
    {
        public ClientsContext(DbContextOptions<ClientsContext> context) : base(context)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(LocalDB)\MSSQLLocalDB;Database=TeledocClientsDb;Trusted_Connection=True");
        }

        public override int SaveChanges()
        {
            BeforeSaving();
            return base.SaveChanges();
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            BeforeSaving();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void BeforeSaving()
        {
            var results = new List<ValidationResult>();
            ChangeTracker.Entries()
                .Where(e => e.State is EntityState.Added or EntityState.Modified)
                .Select(e => e.Entity)
                .OfType<IValidatableObject>()
                .ToList()
                .ForEach(entity =>
                {
                    var results = entity.Validate(new ValidationContext(entity));
                    foreach (var result in results)
                        throw new ValidationException(result.ErrorMessage);
                });

            var now = DateTime.Now;
            ChangeTracker.Entries()
                .Where(e => e.State is EntityState.Added)
                .Select(e => e.Entity)
                .OfType<BaseEntity>()
                .ToList()
                .ForEach(entity =>
                {
                    entity.AdditionDate = now;
                    entity.EditDate = now;
                });
            ChangeTracker.Entries()
                .Where(e => e.State is EntityState.Modified)
                .Select(e => e.Entity)
                .OfType<BaseEntity>()
                .ToList()
                .ForEach(entity => entity.EditDate = now);
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Founder> Founders { get; set; }
    }
}

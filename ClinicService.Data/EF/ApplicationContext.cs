using ClinicService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ClinicService.Data.EF
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Consultation> Consultations { get; set; }

        private string? _connectionString = "Server=.\\SQLEXPRESS;Database=ClinicService;Trusted_Connection=True;MultipleActiveResultSets=True";

        private IConfiguration _configuration;

        private IConfiguration Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    _configuration = ApplicationContextHelper.GetConfiguration();
                }
                return _configuration;
            }
        }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.ConfigureWarnings(x => x.Log((RelationalEventId.CommandExecuting, LogLevel.Debug)));

            var optionExtensionInfo = options.Options.Extensions.FirstOrDefault(f => f.GetType()?.BaseType?.Name.Equals(nameof(RelationalOptionsExtension), StringComparison.InvariantCultureIgnoreCase) == true);
            if (optionExtensionInfo != null) _connectionString = ((RelationalOptionsExtension)optionExtensionInfo).ConnectionString;

            base.OnConfiguring(options);

            // Для миграции, если connectionString не был ранее использован из настроек, то берем закодированнй жестко в классе
            if (!options.IsConfigured)
            {
                ApplicationContextHelper.ConfigureDbContextOptions(options, Configuration);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Consultation>()
                .HasOne(c => c.Pet)
                .WithMany(p => p.Consultations)
                .HasForeignKey(c => c.PetId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
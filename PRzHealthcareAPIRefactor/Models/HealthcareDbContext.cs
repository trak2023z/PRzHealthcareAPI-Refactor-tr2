using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.ReportingServices2010;

namespace PRzHealthcareAPIRefactor.Models
{
    public class HealthcareDbContext : DbContext
    {
        static IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .AddEnvironmentVariables()
                    .Build();
        private string _connectionString = configuration.GetSection("ConnectionString").Value.ToString();

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().HasKey(x => x.Acc_Id);
            modelBuilder.Entity<Account>().Property(x => x.Acc_Login).IsRequired().HasMaxLength(63);
            modelBuilder.Entity<Account>().Property(x => x.Acc_Password).IsRequired().HasMaxLength(2047);
            modelBuilder.Entity<Account>().Property(x => x.Acc_Firstname).IsRequired().HasMaxLength(63);
            modelBuilder.Entity<Account>().Property(x => x.Acc_Lastname).IsRequired().HasMaxLength(63);
            modelBuilder.Entity<Account>().Property(x => x.Acc_DateOfBirth).IsRequired().HasColumnType("date");
            modelBuilder.Entity<Account>().Property(x => x.Acc_Pesel).IsRequired();
            modelBuilder.Entity<Account>().Property(x => x.Acc_Email).HasMaxLength(127);
            modelBuilder.Entity<Account>().Property(x => x.Acc_ContactNumber).HasMaxLength(63);
            modelBuilder.Entity<Account>().Property(x => x.Acc_AtyId).IsRequired();
            modelBuilder.Entity<Account>().Property(x => x.Acc_IsActive).IsRequired();
            modelBuilder.Entity<Account>().Property(x => x.Acc_InsertedDate).IsRequired().HasColumnType("datetime");
            modelBuilder.Entity<Account>().Property(x => x.Acc_ModifiedDate).IsRequired().HasColumnType("datetime");
            modelBuilder.Entity<Account>().Property(x => x.Acc_ReminderExpire).HasColumnType("datetime");
            modelBuilder.Entity<Account>().Property(x => x.Acc_RegistrationHash).HasMaxLength(255);
            modelBuilder.Entity<Account>().Property(x => x.Acc_ReminderHash).HasMaxLength(255);

            modelBuilder.Entity<AccountType>().HasKey(x => x.Aty_Id);
            modelBuilder.Entity<AccountType>().Property(x => x.Aty_Name).IsRequired().HasMaxLength(255);

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}

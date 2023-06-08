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
        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Vaccination> Vaccinations { get; set; }
        public DbSet<BinData> BinData { get; set; }

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

            modelBuilder.Entity<Event>().HasKey(x => x.Eve_Id);
            modelBuilder.Entity<Event>().Property(x => x.Eve_TimeFrom).IsRequired();
            modelBuilder.Entity<Event>().Property(x => x.Eve_TimeTo).IsRequired();
            modelBuilder.Entity<Event>().Property(x => x.Eve_Type).IsRequired();
            modelBuilder.Entity<Event>().Property(x => x.Eve_Description).HasMaxLength(255);
            modelBuilder.Entity<Event>().Property(x => x.Eve_IsActive).IsRequired();
            modelBuilder.Entity<Event>().Property(x => x.Eve_InsertedDate).IsRequired().HasColumnType("datetime");
            modelBuilder.Entity<Event>().Property(x => x.Eve_InsertedAccId).IsRequired();
            modelBuilder.Entity<Event>().Property(x => x.Eve_ModifiedAccId).IsRequired();
            modelBuilder.Entity<Event>().Property(x => x.Eve_ModifiedDate).IsRequired().HasColumnType("datetime");
            modelBuilder.Entity<Event>().Property(x => x.Eve_SerialNumber).HasMaxLength(23);

            modelBuilder.Entity<Vaccination>().HasKey(x => x.Vac_Id);
            modelBuilder.Entity<Vaccination>().Property(x => x.Vac_Name).IsRequired().HasMaxLength(127);
            modelBuilder.Entity<Vaccination>().Property(x => x.Vac_Description).IsRequired().HasMaxLength(2047);
            modelBuilder.Entity<Vaccination>().Property(x => x.Vac_DaysBetweenVacs).IsRequired();
            modelBuilder.Entity<Vaccination>().Property(x => x.Vac_IsActive).IsRequired();
            modelBuilder.Entity<Vaccination>().Property(x => x.Vac_InsertedDate).IsRequired().HasColumnType("datetime");
            modelBuilder.Entity<Vaccination>().Property(x => x.Vac_InsertedAccId).IsRequired();
            modelBuilder.Entity<Vaccination>().Property(x => x.Vac_ModifiedAccId).IsRequired();
            modelBuilder.Entity<Vaccination>().Property(x => x.Vac_ModifiedDate).IsRequired().HasColumnType("datetime");

            modelBuilder.Entity<EventType>().HasKey(x => x.Ety_Id);
            modelBuilder.Entity<EventType>().Property(x => x.Ety_Name).IsRequired().HasMaxLength(127);

            modelBuilder.Entity<BinData>().HasKey(x => x.Bin_Id);
            modelBuilder.Entity<BinData>().Property(x => x.Bin_Name).IsRequired().HasMaxLength(255);
            modelBuilder.Entity<BinData>().Property(x => x.Bin_Data).IsRequired();
            modelBuilder.Entity<BinData>().Property(x => x.Bin_ModifiedAccId).IsRequired();
            modelBuilder.Entity<BinData>().Property(x => x.Bin_ModifiedDate).IsRequired().HasColumnType("datetime");
            modelBuilder.Entity<BinData>().Property(x => x.Bin_InsertedDate).IsRequired().HasColumnType("datetime");
            modelBuilder.Entity<BinData>().Property(x => x.Bin_InsertedAccId).IsRequired();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}

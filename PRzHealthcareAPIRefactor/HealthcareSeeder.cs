using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PRzHealthcareAPIRefactor.Models;

namespace PRzHealthcareAPIRefactor
{
    public class HealthcareSeeder
    {
        private readonly HealthcareDbContext _dbContext;
        private readonly IPasswordHasher<Account> _passwordHasher;

        public HealthcareSeeder(HealthcareDbContext dbContext, IPasswordHasher<Account> passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        public void Seed()
        {
            if (_dbContext.Database.CanConnect())
            {
                var pendingMigration = _dbContext.Database.GetPendingMigrations();
                if (pendingMigration != null && pendingMigration.Any())
                {
                    _dbContext.Database.Migrate();
                }

                if (!_dbContext.AccountTypes.Any())
                {
                    var accountTypes = GetAccountTypes();
                    _dbContext.AccountTypes.AddRange(accountTypes);
                    _dbContext.SaveChanges();
                }
                if (!_dbContext.Accounts.Any())
                {
                    var accounts = GetAccounts();
                    _dbContext.Accounts.AddRange(accounts);
                    _dbContext.SaveChanges();
                }

            }
        }
        private IEnumerable<Account> GetAccounts()
        {
            var accounts = new List<Account>();

            var adminAccountTypeId = _dbContext.AccountTypes.FirstOrDefault(x => x.Aty_Name == "Administrator").Aty_Id;
            var doctorAccountTypeId = _dbContext.AccountTypes.FirstOrDefault(x => x.Aty_Name == "Doktor").Aty_Id;
            var nurseAccountTypeId = _dbContext.AccountTypes.FirstOrDefault(x => x.Aty_Name == "Pielęgniarz").Aty_Id;
            var patientAccountTypeId = _dbContext.AccountTypes.FirstOrDefault(x => x.Aty_Name == "Pacjent").Aty_Id;

            var adminAccount = new Account()
            {
                Acc_AtyId = adminAccountTypeId,
                Acc_ContactNumber = "123-456-789",
                Acc_DateOfBirth = Convert.ToDateTime(DateTime.Now.AddYears(-23).AddMonths(-1)),
                Acc_Email = @"165083@stud.prz.edu.pl",
                Acc_Firstname = "Mateusz",
                Acc_Lastname = "Kubis",
                Acc_Pesel = 98061400000,
                Acc_Secondname = "Dariusz",
                Acc_PhotoId = null,
                Acc_Login = "Administrator",
                Acc_IsActive = true,
                Acc_InsertedDate = DateTime.Now,
                Acc_ModifiedDate = DateTime.Now,
                Acc_RegistrationHash = null,
                Acc_ReminderExpire = null,
                Acc_ReminderHash = null,
            };
            adminAccount.Acc_Password = _passwordHasher.HashPassword(adminAccount, "Leopard12");

            var doctor1Account = new Account()
            {
                Acc_AtyId = doctorAccountTypeId,
                Acc_ContactNumber = "123-456-789",
                Acc_DateOfBirth = Convert.ToDateTime(DateTime.Now.AddYears(-20).AddMonths(2)),
                Acc_Email = @"cmy97956@nezid.com",
                Acc_Firstname = "Jan",
                Acc_Lastname = "Tomczyk",
                Acc_Pesel = 88010125698,
                Acc_Secondname = "",
                Acc_PhotoId = null,
                Acc_Login = "JTomczyk",
                Acc_IsActive = true,
                Acc_InsertedDate = DateTime.Now,
                Acc_ModifiedDate = DateTime.Now,
                Acc_RegistrationHash = null,
                Acc_ReminderExpire = null,
                Acc_ReminderHash = null,
            };
            doctor1Account.Acc_Password = _passwordHasher.HashPassword(doctor1Account, "Leopard12");

            var doctor2Account = new Account()
            {
                Acc_AtyId = doctorAccountTypeId,
                Acc_ContactNumber = "966-455-123",
                Acc_DateOfBirth = Convert.ToDateTime(DateTime.Now.AddYears(-40).AddMonths(4)),
                Acc_Email = @"mri78421@zslsz.com",
                Acc_Firstname = "Andrzej",
                Acc_Lastname = "Rodzinny",
                Acc_Pesel = 68052369584,
                Acc_Secondname = "Tomasz",
                Acc_PhotoId = null,
                Acc_Login = "ARodzinny",
                Acc_IsActive = true,
                Acc_InsertedDate = DateTime.Now,
                Acc_ModifiedDate = DateTime.Now,
                Acc_RegistrationHash = null,
                Acc_ReminderExpire = null,
                Acc_ReminderHash = null,
            };
            doctor2Account.Acc_Password = _passwordHasher.HashPassword(doctor2Account, "Leopard12");

            var nurseAccount = new Account()
            {
                Acc_AtyId = nurseAccountTypeId,
                Acc_ContactNumber = "123-456-789",
                Acc_DateOfBirth = Convert.ToDateTime(DateTime.Now.AddYears(-30).AddMonths(-5)),
                Acc_Email = @"qft83545@omeie.com",
                Acc_Firstname = "Marta",
                Acc_Lastname = "Nowak",
                Acc_Pesel = 96021456325,
                Acc_Secondname = "",
                Acc_PhotoId = null,
                Acc_Login = "MNowak",
                Acc_IsActive = true,
                Acc_InsertedDate = DateTime.Now,
                Acc_ModifiedDate = DateTime.Now,
                Acc_RegistrationHash = null,
                Acc_ReminderExpire = null,
                Acc_ReminderHash = null,
            };
            nurseAccount.Acc_Password = _passwordHasher.HashPassword(nurseAccount, "Leopard12");

            var patientAccount = new Account()
            {
                Acc_AtyId = patientAccountTypeId,
                Acc_ContactNumber = "987-456-132",
                Acc_DateOfBirth = Convert.ToDateTime(DateTime.Now.AddYears(-19).AddMonths(1)),
                Acc_Email = @"zmn98802@nezid.com",
                Acc_Firstname = "Mariola",
                Acc_Lastname = "Łoskot",
                Acc_Pesel = 96060136548,
                Acc_Secondname = "",
                Acc_PhotoId = null,
                Acc_Login = "MLoskot",
                Acc_IsActive = true,
                Acc_InsertedDate = DateTime.Now,
                Acc_ModifiedDate = DateTime.Now,
                Acc_RegistrationHash = null,
                Acc_ReminderExpire = null,
                Acc_ReminderHash = null,
            };
            patientAccount.Acc_Password = _passwordHasher.HashPassword(patientAccount, "Leopard12");

            accounts.Add(patientAccount);
            accounts.Add(nurseAccount);
            accounts.Add(doctor1Account);
            accounts.Add(doctor2Account);
            accounts.Add(adminAccount);

            return accounts;
        }
        public IEnumerable<AccountType> GetAccountTypes()
        {
            var accountTypes = new List<AccountType>()
            {
                new AccountType()
                {
                    Aty_Name = "Pacjent"
                },
                new AccountType()
                {
                    Aty_Name = "Pielęgniarz"
                },
                new AccountType()
                {
                    Aty_Name = "Doktor"
                },
                new AccountType()
                {
                    Aty_Name = "Administrator"
                },
                new AccountType()
                {
                    Aty_Name = "Niepotwierdzony"
                }
            };

            return accountTypes;
        }
    }
}

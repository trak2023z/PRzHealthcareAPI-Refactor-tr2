using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PRzHealthcareAPIRefactor.Exceptions;
using PRzHealthcareAPIRefactor.Helpers;
using PRzHealthcareAPIRefactor.Models;
using PRzHealthcareAPIRefactor.Models.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PRzHealthcareAPIRefactor.Services
{
    public interface IUserService
    {
        void Register(RegisterUserDto dto);
        LoginUserDto? GenerateToken(LoginUserDto dto);
        List<UserDto> GetDoctorsList();
        List<UserDto> GetPatientsList();
    }

    public class UserService : IUserService
    {
        private readonly HealthcareDbContext _dbContext;
        private readonly AuthenticationSettings _authentication;
        private readonly IPasswordHasher<Account> _passwordHasher;
        private readonly IMapper _mapper;

        public UserService(HealthcareDbContext dbContext, AuthenticationSettings authentication, IPasswordHasher<Account> passwordHasher, IMapper mapper)
        {
            _dbContext = dbContext;
            _authentication = authentication;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
        }

        /// <summary>
        /// Rejestracja użytkownika w systemie
        /// </summary>
        /// <param name="dto">Obiekt użytkownika</param>
        /// <returns>Poprawność wykonania funkcji</returns>
        public void Register(RegisterUserDto dto)
        {
            try
            {
                var loginExists = _dbContext.Accounts.Any(x => x.Acc_Login == dto.Login || x.Acc_Email == dto.Email || x.Acc_Pesel == dto.Pesel);
                if (loginExists)
                {
                    throw new BadRequestException("Konto o tym loginie, adresie e-mail lub peselu już istnieje.");
                }

                var newUser = _mapper.Map<Account>(dto);
                newUser.Acc_RegistrationHash = Tools.CreateRandomToken(64);
                newUser.Acc_InsertedDate = DateTime.Now;
                newUser.Acc_ModifiedDate = DateTime.Now;
                newUser.Acc_IsActive = true;
                newUser.Acc_PhotoId = null;
                newUser.Acc_Password = _passwordHasher.HashPassword(newUser, dto.Password);
                newUser.Acc_AtyId = _dbContext.AccountTypes.FirstOrDefault(x => x.Aty_Name == "Niepotwierdzony").Aty_Id;
                _dbContext.Accounts.Add(newUser);
                _dbContext.SaveChanges();

            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
        }
        /// <summary>
        /// Logowanie użytkownika - generowanie tokenu
        /// </summary>
        /// <param name="dto">Obiekt użytkownika</param>
        /// <returns>Obiekt użytkownika z tokenem</returns>
        public LoginUserDto? GenerateToken(LoginUserDto dto)
        {
            var user = _dbContext.Accounts
                .Include(r => r.AccountType)
                .FirstOrDefault(x => x.Acc_Login == dto.Login);

            if (user is null)
            {
                throw new BadRequestException("Błędny login lub hasło.");
            }

            if (_passwordHasher.VerifyHashedPassword(user, user.Acc_Password, dto.Password) == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("Błędny login lub hasło.");
            }

            /*  Niezarejestrowany   */
            if (user.AccountType.Aty_Name == "Niepotwierdzony")
            {
                throw new BadRequestException("Twoje konto nie zostało wciąż potwierdzone.");
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Acc_Login.ToString()),
                new Claim(ClaimTypes.Name, $@"{user.Acc_Firstname} {user.Acc_Lastname}"),
                new Claim(ClaimTypes.Role, user.AccountType.Aty_Name.ToString()),
                new Claim(ClaimTypes.SerialNumber, user.Acc_Id.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authentication.JwtKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddHours(_authentication.JwtExpireHours);

            var token = new JwtSecurityToken(_authentication.JwtIssuer, _authentication.JwtIssuer, claims, expires: expires, signingCredentials: cred);

            var tokenHandler = new JwtSecurityTokenHandler();

            LoginUserDto loginUser = new()
            {
                AccId = user.Acc_Id,
                Login = dto.Login,
                Name = user.Acc_Firstname,
                AtyId = user.Acc_AtyId,
                Token = tokenHandler.WriteToken(token)
            };

            return loginUser;
        }

        /// <summary>
        /// Pobranie listy doktorów
        /// </summary>
        /// <returns>Lista doktorów</returns>
        public List<UserDto> GetDoctorsList()
        {
            var doctorAccountTypeId = _dbContext.AccountTypes.FirstOrDefault(x => x.Aty_Name == "Doktor").Aty_Id;
            var list = _dbContext.Accounts.Where(x => x.Acc_AtyId == doctorAccountTypeId && x.Acc_IsActive).ToList();
            if (list is null)
            {
                return new List<UserDto>();
            }

            List<UserDto> listUserDto = new();

            foreach (var account in list)
            {
                listUserDto.Add(_mapper.Map<UserDto>(account));
            }

            return listUserDto;
        }
        /// <summary>
        /// Pobranie listy pacjentów
        /// </summary>
        /// <returns>Lista obiektów pacjentów</returns>
        public List<UserDto> GetPatientsList()
        {
            var patientAccountTypeId = _dbContext.AccountTypes.FirstOrDefault(x => x.Aty_Name == "Pacjent").Aty_Id;
            var list = _dbContext.Accounts.Where(x => x.Acc_AtyId == patientAccountTypeId && x.Acc_IsActive).ToList();
            if (list is null)
            {
                return new List<UserDto>();
            }

            List<UserDto> listUserDto = new();

            foreach (var account in list)
            {
                listUserDto.Add(_mapper.Map<UserDto>(account));
            }

            return listUserDto;
        }
    }
}

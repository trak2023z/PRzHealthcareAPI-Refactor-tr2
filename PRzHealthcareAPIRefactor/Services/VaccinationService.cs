using AutoMapper;
using Microsoft.AspNetCore.Identity;
using PRzHealthcareAPIRefactor.Models;
using PRzHealthcareAPIRefactor.Models.DTO;

namespace PRzHealthcareAPIRefactor.Services
{
    public interface IVaccinationService
    {
        List<VaccinationDto> GetAll();
    }

    public class VaccinationService : IVaccinationService
    {
        private readonly HealthcareDbContext _dbContext;
        private readonly AuthenticationSettings _authentication;
        private readonly IPasswordHasher<Account> _passwordHasher;
        private readonly IMapper _mapper;

        public VaccinationService(HealthcareDbContext dbContext, AuthenticationSettings authentication, IPasswordHasher<Account> passwordHasher, IMapper mapper)
        {
            _dbContext = dbContext;
            _authentication = authentication;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
        }

        /// <summary>
        /// Pobranie listy szczepionek
        /// </summary>
        /// <returns>Lista obiektów szczepionek</returns>
        public List<VaccinationDto> GetAll()
        {
            var vaccinationList = _dbContext.Vaccinations.ToList();
            if (vaccinationList is null)
            {
                return new List<VaccinationDto>();
            }

            List<VaccinationDto> vaccinationListDto = new List<VaccinationDto>();

            foreach (var vaccination in vaccinationList)
            {
                vaccinationListDto.Add(_mapper.Map<VaccinationDto>(vaccination));
            }

            return vaccinationListDto;
        }
    }
}

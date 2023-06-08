using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PRzHealthcareAPIRefactor.Exceptions;
using PRzHealthcareAPIRefactor.Helpers;
using PRzHealthcareAPIRefactor.Models;
using PRzHealthcareAPIRefactor.Models.DTO;
using PublicHoliday;

namespace PRzHealthcareAPIRefactor.Services
{
    public interface IEventService
    {
        void TakeTerm(EventDto dto, string accountId);
        void FinishTerm(EventDto dto, string accountId);
        bool SeedDates(int doctorId);
    }

    public class EventService : IEventService
    {
        private readonly HealthcareDbContext _dbContext;
        private readonly AuthenticationSettings _authentication;
        private readonly IMapper _mapper;
        private readonly ICertificateService _certificateService;

        public EventService(HealthcareDbContext dbContext, AuthenticationSettings authentication, IMapper mapper, ICertificateService certificateService)
        {
            _dbContext = dbContext;
            _authentication = authentication;
            _mapper = mapper;
            _certificateService = certificateService;
        }

        /// <summary>
        /// Zajęcie terminu
        /// </summary>
        /// <param name="dto">Obiekt danego terminu</param>
        /// <param name="accountId">Id zalogowanego użytkownika</param>
        public void TakeTerm(EventDto dto, string accountId)
        {
            var busyEventTypeId = _dbContext.EventTypes.FirstOrDefault(x => x.Ety_Name == "Zajęty").Ety_Id;
            dto.DateFrom = Convert.ToDateTime(dto.DateFrom).AddHours(2);
            TimeSpan timeFromSpan = TimeSpan.Parse(dto.TimeFrom);
            dto.DateFrom = Convert.ToDateTime(dto.DateFrom).Add(timeFromSpan);

            var changedEvent = _dbContext.Events.FirstOrDefault(x => x.Eve_TimeFrom == dto.DateFrom && x.Eve_DoctorId == dto.DoctorId);
            if (changedEvent == null)
            {
                throw new NotFoundException("Nie znaleziono odpowiedniego terminu. W razie problemów prosimy o kontakt telefoniczny.");
            }
            Account user = new Account();
            if (dto.AccId == 0)
            {
                user = _dbContext.Accounts.FirstOrDefault(x => x.Acc_Id.ToString() == accountId);
            }
            else
            {
                user = _dbContext.Accounts.FirstOrDefault(x => x.Acc_Id == dto.AccId);
            }
            if (user == null)
            {
                throw new NotFoundException("Nie znaleziono użytkownika.");
            }
            if (changedEvent.Eve_Type == busyEventTypeId)
            {
                throw new NotFoundException("Termin został zajęty.");
            }

            var selectedVaccination = _dbContext.Vaccinations.FirstOrDefault(x => x.Vac_Id == dto.VacId);
            var finishEventTypeId = _dbContext.EventTypes.FirstOrDefault(x => x.Ety_Name == "Zakończony").Ety_Id;
            var lastUserVaccination = _dbContext.Events.Where(x => x.Eve_AccId == user.Acc_Id && x.Eve_Type == finishEventTypeId).ToList();

            if (lastUserVaccination.Any(x => x.Eve_TimeFrom.AddDays(selectedVaccination.Vac_DaysBetweenVacs) > Convert.ToDateTime(dto.TimeFrom)))
            {
                throw new BadRequestException("Prosimy odczekać odstęp czasu opisany w zaświadczeniu. W razie problemów prosimy o kontakt telefoniczny.");
            }

            var lastUserVaccinationRequest = _dbContext.Events.Where(x => x.Eve_AccId == user.Acc_Id && x.Eve_Type == busyEventTypeId).ToList();
            if (lastUserVaccinationRequest.Any())
            {
                throw new BadRequestException("Nie ma możliwości rejestracji na dwie oddzielne wizyty.");
            }

            changedEvent.Eve_AccId = user.Acc_Id;
            changedEvent.Eve_Type = busyEventTypeId;
            changedEvent.Eve_DoctorId = dto.DoctorId;
            changedEvent.Eve_VacId = dto.VacId;
            changedEvent.Eve_Description = "Szczepienie";
            changedEvent.Eve_ModifiedAccId = Convert.ToInt32(accountId);
            changedEvent.Eve_ModifiedDate = DateTime.Now;
            changedEvent.Eve_InsertedDate = DateTime.Now;
            changedEvent.Eve_InsertedAccId = Convert.ToInt32(accountId);


            _dbContext.Update(changedEvent);
            _dbContext.SaveChanges();

        }

        /// <summary>
        /// Zakończenie terminu
        /// </summary>
        /// <param name="dto">Obiekt terminu</param>
        /// <param name="accountId">Id zalogowanego użytkownika</param>
        public void FinishTerm(EventDto dto, string accountId)
        {
            var finishEventTypeId = _dbContext.EventTypes.FirstOrDefault(x => x.Ety_Name == "Zakończony").Ety_Id;
            var finishedEvent = _dbContext.Events.FirstOrDefault(x => x.Eve_Id == dto.Id);
            if (finishedEvent == null)
            {
                throw new NotFoundException("Wydarzenie nie istnieje.");
            }
            var user = _dbContext.Accounts.FirstOrDefault(x => x.Acc_Id == dto.AccId);
            if (user == null)
            {
                throw new NotFoundException("Nie znaleziono użytkownika.");
            }

            finishedEvent.Eve_Type = finishEventTypeId;
            finishedEvent.Eve_ModifiedAccId = Convert.ToInt32(accountId);
            finishedEvent.Eve_ModifiedDate = DateTime.Now;
            finishedEvent.Eve_SerialNumber = Tools.CreateRandomToken(5);

            _dbContext.Update(finishedEvent);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Zapełnienie wolnych terminów
        /// </summary>
        /// <returns>Poprawność wykonania funkcji</returns>
        public bool SeedDates(int doctorId)
        {
            var calendar = new PolandPublicHoliday();

            var doctorAccountTypeId = _dbContext.AccountTypes.FirstOrDefault(x => x.Aty_Name == "Doktor").Aty_Id;
            var administratorAccountId = _dbContext.Accounts.FirstOrDefault(x => x.Acc_Login == "Administrator").Acc_Id;
            var availableEventTypeId = _dbContext.EventTypes.FirstOrDefault(x => x.Ety_Name == "Wolny").Ety_Id;

            var doctorsList = _dbContext.Accounts.Where(x => x.Acc_AtyId == doctorAccountTypeId).ToList();
            if (!doctorsList.Any())
            {
                throw new NotFoundException($@"Uzupełnij listę doktorów.");
            }
            int startHour = 8;
            int endHour = 16;

            for (int i = 0; i < 30; i++)
            {

                int actualHour = startHour;
                int actualMinute = 0;

                while (actualHour < endHour)
                {
                    DateTime actualDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(i).Day, actualHour, actualMinute, 0);

                    if (!calendar.IsPublicHoliday(actualDate))
                    {
                        if (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(i).Day, 0, 0, 0).DayOfWeek != DayOfWeek.Saturday)
                        {
                            if (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(i).Day, 0, 0, 0).DayOfWeek != DayOfWeek.Sunday)
                            {
                                var eventsList = _dbContext.Events.Where(x => x.Eve_DoctorId == doctorId && x.Eve_TimeFrom == actualDate).ToList();

                                if (!eventsList.Any())
                                {
                                    Event seedEvent = new Event();
                                    seedEvent = new Event()
                                    {
                                        Eve_AccId = null,
                                        Eve_TimeFrom = actualDate,
                                        Eve_TimeTo = actualDate.AddMinutes(15),
                                        Eve_Description = "Dostępny",
                                        Eve_InsertedAccId = administratorAccountId,
                                        Eve_InsertedDate = DateTime.Now,
                                        Eve_DoctorId = doctorId,
                                        Eve_IsActive = true,
                                        Eve_ModifiedAccId = administratorAccountId,
                                        Eve_ModifiedDate = DateTime.Now,
                                        Eve_Type = availableEventTypeId,
                                        Eve_VacId = null,
                                    };

                                    _dbContext.Events.Add(seedEvent);
                                    _dbContext.SaveChanges();
                                }

                                actualMinute += 15;

                                if (actualMinute == 60)
                                {
                                    actualMinute = 0;
                                    actualHour++;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            for (int i = 0; i < 30; i++)
            {

                int actualHour = startHour;
                int actualMinute = 0;

                while (actualHour < endHour)
                {
                    DateTime actualDate = new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(1).Month, DateTime.Now.AddDays(i).Day, actualHour, actualMinute, 0);

                    if (!calendar.IsPublicHoliday(actualDate))
                    {
                        if (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(i).Day, 0, 0, 0).DayOfWeek != DayOfWeek.Saturday)
                        {
                            if (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(i).Day, 0, 0, 0).DayOfWeek != DayOfWeek.Sunday)
                            {
                                var eventsList = _dbContext.Events.Where(x => x.Eve_DoctorId == doctorId && x.Eve_TimeFrom >= DateTime.Now.AddDays(i)).ToList();

                                if (!eventsList.Any(x => x.Eve_TimeFrom == actualDate && x.Eve_DoctorId == doctorId))
                                {
                                    Event seedEvent = new Event();
                                    seedEvent = new Event()
                                    {
                                        Eve_AccId = null,
                                        Eve_TimeFrom = actualDate,
                                        Eve_TimeTo = actualDate.AddMinutes(15),
                                        Eve_Description = "Dostępny",
                                        Eve_InsertedAccId = administratorAccountId,
                                        Eve_InsertedDate = DateTime.Now,
                                        Eve_DoctorId = doctorId,
                                        Eve_IsActive = true,
                                        Eve_ModifiedAccId = administratorAccountId,
                                        Eve_ModifiedDate = DateTime.Now,
                                        Eve_Type = availableEventTypeId,
                                        Eve_VacId = null,
                                    };

                                    _dbContext.Events.Add(seedEvent);
                                    _dbContext.SaveChanges();
                                }

                                actualMinute += 15;

                                if (actualMinute == 60)
                                {
                                    actualMinute = 0;
                                    actualHour++;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return true;

        }
    }
}

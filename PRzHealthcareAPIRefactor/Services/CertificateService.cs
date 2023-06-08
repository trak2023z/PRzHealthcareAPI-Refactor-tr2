using BoldReports.Writer;
using Microsoft.AspNetCore.Mvc;
using PRzHealthcareAPIRefactor.Exceptions;
using PRzHealthcareAPIRefactor.Models;
using PRzHealthcareAPIRefactor.Models.DTO;

namespace PRzHealthcareAPIRefactor.Services
{
    public interface ICertificateService
    {
        FileStreamResult PrintCOVIDCertificateToPDF(int eventId);
    }

    public class CertificateService : ICertificateService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly HealthcareDbContext _dbContext;

        public CertificateService(IWebHostEnvironment hostingEnvironment, HealthcareDbContext dbContext)
        {
            _hostingEnvironment = hostingEnvironment;
            _dbContext = dbContext;

        }

        /// <summary>
        /// Wydruk zaświadczenia COVID do pliku PDF
        /// </summary>
        /// <param name="eventId">Identyfikator eventu</param>
        /// <returns>Plik z wygenerowaniem wydrukiem</returns>
        /// <exception cref="BadRequestException">Błąd podczas próby wydruku</exception>
        public FileStreamResult PrintCOVIDCertificateToPDF(int eventId)
        {
            try
            {
                var baseCode = _dbContext.BinData.FirstOrDefault(x => x.Bin_Id == 1).Bin_Data;
                byte[] fileBytes = Convert.FromBase64String(baseCode);

                using MemoryStream stream = new(fileBytes);
                ReportWriter writer = new()
                {
                    ReportProcessingMode = ProcessingMode.Remote
                };
                List<BoldReports.Web.ReportParameter> userParameters = new() { new BoldReports.Web.ReportParameter() { Name = "EventId", Values = new List<string>() { eventId.ToString() } } };

                string fileName = "ZaswiadczenieCOVID.pdf";
                string type = "pdf";
                WriterFormat format = WriterFormat.PDF;

                writer.LoadReport(stream);
                writer.SetParameters(userParameters);

                MemoryStream memoryStream = new();
                writer.Save(memoryStream, format);
                memoryStream.Position = 0;

                FileStreamResult fileStreamResult = new(memoryStream, "application/" + type)
                {
                    FileDownloadName = fileName
                };

                return fileStreamResult;

            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
        }
    }
}

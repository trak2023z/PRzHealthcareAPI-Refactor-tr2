using System.Security.Cryptography;

namespace PRzHealthcareAPIRefactor.Helpers
{
    public class Tools
    {
        /// <summary>
        /// Generowanie ciągu znaków
        /// </summary>
        /// <param name="bytes">Ilość bajtów do wygenerowania tekstu</param>
        /// <returns></returns>
        public static string CreateRandomToken(int bytes)
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(bytes));
        }
        /// <summary>
        /// Konwersja pliku do BASE64
        /// </summary>
        /// <param name="filePath">Ścieżka do pliku</param>
        /// <returns></returns>
        public static string ToBase64Converter(string filePath)
        {
            string daneBinarne;
            if (System.IO.File.Exists(filePath))
            {
                byte[] bufor = System.IO.File.ReadAllBytes(filePath);
                daneBinarne = System.Convert.ToBase64String(bufor);
            }
            else
            {
                return "";
            }
            return daneBinarne;
        }

        /// <summary>
        /// Konwersja BASE64 do pliku
        /// </summary>
        /// <param name="baseCode">kod BASE64</param>
        /// <returns></returns>
        public static string FromBase64Converter(IWebHostEnvironment hostingEnvironment, string baseCode)
        {
            try
            {
                string path = Path.Combine(hostingEnvironment.ContentRootPath, "wydruk.rpt");
                if (File.Exists(path))
                {
                    using FileStream stream = new(Path.Combine(hostingEnvironment.ContentRootPath, "wydruk.rpt"), FileMode.Open);
                    File.Delete(path);
                    stream.Dispose();
                }


                byte[] tempBytes = Convert.FromBase64String(baseCode);
                File.WriteAllBytes(path, tempBytes);

                return path;
            }
            catch
            {
                return "";
            }
        }
    }
}

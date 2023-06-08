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
    }
}

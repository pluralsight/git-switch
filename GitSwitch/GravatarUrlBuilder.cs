using System;
using System.Security.Cryptography;
using System.Text;

namespace GitSwitch
{
    public interface IGravatarUrlBuilder
    {
        string HashEmail(string email);
        string GetUrlForEmail(string email);
    }

    public class GravatarUrlBuilder : IGravatarUrlBuilder
    {
        public string HashEmail(string email)
        {
            return Md5(NormalizeEmail(email));
        }

        string Md5(string email)
        {
            using (var md5 = MD5.Create())
            {
                var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(email));

                return BitConverter.ToString(bytes)
                    .Replace("-", string.Empty)
                    .ToLower();
            }
        }

        internal string NormalizeEmail(string email)
        {
            return email.Trim().ToLower();
        }

        public string GetUrlForEmail(string email)
        {
            return string.Format(AppConstants.GravatarUrlFormat, HashEmail(email));
        }
    }
}

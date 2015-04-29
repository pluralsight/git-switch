using System;
using System.Security.Cryptography;
using System.Text;

namespace GitSwitch
{
    public class GravatarUrlBuilder
    {
        public string NormalizeEmail(string email)
        {
            return email.Trim().ToLower();
        }

        public string HashEmail(string normalizedEmail)
        {
            var hashBytes = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(normalizedEmail));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        public string GetUrlForEmail(string email)
        {
            var hash = HashEmail(NormalizeEmail(email));
            return string.Format("http://gravatar.com/avatar/{0}?s=64&r=g", hash);
        }
    }
}

using System;

namespace ShellApp.Helpers
{
    public class Base64StringHelper
    {
        public static byte[] DecodeBase64String(string inputString)
        {
            inputString = inputString.Replace('-', '+').Replace('_', '/').PadRight(4 * ((inputString.Length + 3) / 4), '=');
            return Convert.FromBase64String(inputString);
        }
    }
}

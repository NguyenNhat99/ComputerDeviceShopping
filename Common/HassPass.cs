using System.Security.Cryptography;
using System.Text;

public class HassPass
{
    public static string HassPassSHA512(string password)
    {
        string result = "";
        using (SHA512 sha512 = SHA512.Create())
        {
            byte[] convert = Encoding.UTF8.GetBytes(password);
            byte[] hasspwd = sha512.ComputeHash(convert);
            result = BitConverter.ToString(hasspwd);
        }
        return result;
    }
}

using System.Security.Cryptography;
using System.Text;

public static class Criptografia
{
    public static string GerarHash(string senha)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(senha);
        var hash = sha.ComputeHash(bytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
}

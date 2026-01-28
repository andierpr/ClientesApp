using BCrypt.Net;

namespace ClientesApp.Helpers
{
    public static class Criptografia
    {
        private const int WORK_FACTOR = 12;

        public static string GerarHash(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha))
                throw new ArgumentException("Senha inválida");

            return BCrypt.Net.BCrypt.HashPassword(senha.Trim(), workFactor: WORK_FACTOR);
        }

        public static bool Verificar(string senhaDigitada, string hashArmazenado)
        {
            if (string.IsNullOrWhiteSpace(senhaDigitada) || string.IsNullOrWhiteSpace(hashArmazenado))
                return false;

            try
            {
                return BCrypt.Net.BCrypt.Verify(senhaDigitada.Trim(), hashArmazenado);
            }
            catch
            {
                return false;
            }
        }
    }
}

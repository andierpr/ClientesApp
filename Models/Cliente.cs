namespace ClientesApp.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        public string Nome { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Telefone { get; set; }

        public int? IdEstado { get; set; }

        public Estado? Estado { get; set; }


        // 🔹 Propriedades auxiliares para exibição em Views
        public string TelefoneExibicao => string.IsNullOrWhiteSpace(Telefone) ? "—" : Telefone;

        public string NomeEstadoExibicao => Estado?.NomeEstado ?? "—";

        public string UFExibicao => Estado?.UF ?? "—";
    }
}

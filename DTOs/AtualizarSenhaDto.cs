using System.ComponentModel.DataAnnotations;

namespace ClientesApp.Models
{
    public class AtualizarSenhaDto
    {
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string NovaSenha { get; set; } = string.Empty;
    }
}

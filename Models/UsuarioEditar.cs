using ClientesApp.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClientesApp.Models
{
    public class UsuarioEditar
    {
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        public string? Login { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string Email { get; set; } = string.Empty;

        public bool Ativo { get; set; }

        [Required]
        public UserRole Role { get; set; } = UserRole.Usuario;

        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string? NovaSenha { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Senha")]
        [Compare("NovaSenha", ErrorMessage = "A senha e a confirmação não coincidem.")]
        public string? ConfirmarSenha { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace ClientesApp.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required]
        [StringLength(150)]
        public string Nome { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Login { get; set; } // pode ser nulo

        [Required]
        [StringLength(120)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string SenhaHash { get; set; } = string.Empty;

        public bool Ativo { get; set; } = true;

        public DateTime? UltimoAcesso { get; set; }

        public DateTime CriadoEm { get; set; } = DateTime.Now;

        public DateTime? UltimoLogin { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ClientesApp.Enums;

namespace ClientesApp.Models;

public class Usuario
{
    [Key]
    public int IdUsuario { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Login { get; set; }

    [Required(ErrorMessage = "O e-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    [StringLength(120)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória")]
    [StringLength(255)]
    public string SenhaHash { get; set; } = string.Empty;

    // Controle de permissão
    [Required]
    [StringLength(50)]
    [RegularExpression(
        "^(Usuario|Administrador)$",
        ErrorMessage = "Role inválida. Use 'Usuario' ou 'Administrador'."
    )]
    public string Role { get; set; } = UserRole.Usuario.ToString();

    public bool Ativo { get; set; } = true;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public DateTime? UltimoAcesso { get; set; }

    public DateTime? UltimoLogin { get; set; }

    // 🔁 Enum auxiliar (não mapeado no banco)
    [NotMapped]
    public UserRole RoleEnum
    {
        get => Enum.Parse<UserRole>(Role);
        set => Role = value.ToString();
    }
}

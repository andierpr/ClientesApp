using System.ComponentModel.DataAnnotations;

namespace ClientesApp.Models
{
    public class Estado
    {
        public int IdEstado { get; set; }

        [Required(ErrorMessage = "A sigla do estado é obrigatória.")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "A sigla do estado deve ter exatamente 2 caracteres.")]
        public string UF { get; set; } = null!;
        [Required(ErrorMessage = "O nome do estado é obrigatório.")]
        [StringLength(100)]
        public string NomeEstado { get; set; } = null!;

        public ICollection<Cliente>? Clientes { get; set; }
    }
}



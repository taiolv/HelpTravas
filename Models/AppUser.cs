using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SuporteIA.Models
{
    // 🔹 Classe base para autenticação e perfis
    public class AppUser : IdentityUser
    {
        [Required(ErrorMessage = "O nome completo é obrigatório")]
        [StringLength(100)]
        public string NomeCompleto { get; set; }

        // 🔹 Você pode adicionar outros campos personalizados:
        public string? Cargo { get; set; }  // Exemplo: Técnico, Usuário, etc.
        public DateTime DataCadastro { get; set; } = DateTime.Now;
    }
}

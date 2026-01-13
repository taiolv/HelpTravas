using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuporteIA.Models
{
    public class Equipe
    {
        [Key]
        [Display(Name = "Código da Equipe")]
        public int CdEquipe { get; set; }
        [Required(ErrorMessage = "O nome da equipe é obrigatório.")]
        [StringLength(100)]
        [Display(Name = "Nome da Equipe")]
        public string NmEquipe { get; set; } = string.Empty;
        [ForeignKey("Departamento")]
        [Display(Name = "Departamento")]
        public int FkCdDepartamento { get; set; }
        public Departamento? Departamento { get; set; }
    }
}

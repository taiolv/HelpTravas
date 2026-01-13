using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SuporteIA.Models
{
    public class Departamento
    {
        [Key]
        [Display(Name = "Código do Departamento")]
        public int CdDepartamento { get; set; }
        [Required(ErrorMessage = "O nome do departamento é obrigatório.")]
        [StringLength(100)]
        [Display(Name = "Nome do Departamento")]
        public string NmDepartamento { get; set; }
        [Display(Name = "Descrição do Departamento")]
        [StringLength(255)]
        public string? DsDepartamento { get; set; }
        [Display(Name = "Relacionamento")]
        public string? NmRelacionamento { get; set; }
        public ICollection<Equipe> Equipes { get; set; } = new List<Equipe>();
    }
}

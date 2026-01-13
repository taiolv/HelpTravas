using System;
using System.ComponentModel.DataAnnotations;

namespace SuporteIA.Models
{
    public class Faq
    {
        [Key]
        [Display(Name = "Código")]
        public int Id { get; set; }
        [Required(ErrorMessage = "A pergunta é obrigatória.")]
        [StringLength(255, ErrorMessage = "A pergunta deve ter no máximo 255 caracteres.")]
        [Display(Name = "Pergunta")]
        public string Pergunta { get; set; } = string.Empty;
        [Required(ErrorMessage = "A resposta é obrigatória.")]
        [Display(Name = "Resposta")]
        public string Resposta { get; set; } = string.Empty;
        [Display(Name = "Data de Criação")]
        [DataType(DataType.DateTime)]
        public DateTime CriadoEm { get; set; } = DateTime.Now;
        [Display(Name = "Fonte")]
        [StringLength(50)]
        public string? Fonte { get; set; }
    }
}

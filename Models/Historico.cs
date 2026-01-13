using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuporteIA.Models
{
    public class Historico
    {
        [Key]
        public int CdHistorico { get; set; }
        [ForeignKey(nameof(Chamado))]
        [Display(Name = "Chamado")]
        public int FkCdChamado { get; set; }
        public Chamado Chamado { get; set; }
        [Required]
        [Display(Name = "Mensagem Arquivada")]
        public string MensagemArquivada { get; set; } = string.Empty;
        [Display(Name = "Mensagem de Solução")]
        public string? FkMensagemSolucao { get; set; }
        [Display(Name = "Status no Momento")]
        public string? FkNmStatus { get; set; }
        [Display(Name = "Data do Registro")]
        public DateTime DataRegistro { get; set; } = DateTime.Now;
    }
}

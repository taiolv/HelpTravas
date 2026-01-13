using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SuporteIA.Models
{
    public class Chamado
    {
        [Key]
        public int CdChamado { get; set; }

        [Required, Display(Name = "Descrição do problema")]
        public string NmProblema { get; set; } = string.Empty;

        // Preenchidos manualmente ou pela IA/operador
        public string? Categoria { get; set; }
        public string? Status { get; set; } = "Aberto";
        public string? Prioridade { get; set; } = "Normal";

        // Vínculo ao solicitante (LGPD-friendly)
        public string? SolicitanteUserId { get; set; }
        public string? SolicitanteNome { get; set; }

        // 🔹 Data e hora de abertura do chamado
        [Display(Name = "Data de abertura")]
        [DataType(DataType.DateTime)]
        public DateTime DataAbertura { get; set; } = DateTime.Now;

        // Resposta da IA
        [Display(Name = "Sugestão da IA")]
        public string? SolucaoIA { get; set; }

        // 🔹 Relacionamento com histórico (1 Chamado → vários históricos)
        public List<Historico> Historicos { get; set; } = new();
    }
}

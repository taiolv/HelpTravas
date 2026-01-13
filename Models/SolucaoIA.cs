using System.ComponentModel.DataAnnotations;

namespace SuporteIA.Models
{
    public class SolucaoIA
    {
        [Key]
        public int CdSolucao { get; set; }
        public int FkCdCategoria { get; set; }
        public string MensagemSolucao { get; set; }
    }
}

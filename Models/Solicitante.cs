using System.ComponentModel.DataAnnotations;

namespace SuporteIA.Models
{
    public class Solicitante
    {
        [Key]
        public int CdSolicitante { get; set; }
        public string NmSolicitante { get; set; }
    }
}

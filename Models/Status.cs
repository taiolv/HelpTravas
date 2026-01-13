using System.ComponentModel.DataAnnotations;

namespace SuporteIA.Models
{
    public class Status
    {
        [Key]
        public int IdStatus { get; set; }
        public string NmStatus { get; set; }
    }
}

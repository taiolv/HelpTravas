using System.ComponentModel.DataAnnotations;

namespace SuporteIA.Models
{
    public class IaAuditLog
    {
        [Key] public int Id { get; set; }
        [Required] public string Provider { get; set; } = "OpenRouter";
        [Required] public string Prompt { get; set; } = "";
        public string? Response { get; set; }
        public string? Error { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? UserId { get; set; }
    }
}

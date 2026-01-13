using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SuporteIA.Models;

namespace SuporteIA.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Chamado> Chamados { get; set; }
        public DbSet<Solicitante> Solicitantes { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Equipe> Equipes { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Historico> Historicos { get; set; }
        public DbSet<SolucaoIA> SolucoesIA { get; set; }
        public DbSet<Faq> Faqs { get; set; }
        public DbSet<IaAuditLog> IaAuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach (var fk in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}

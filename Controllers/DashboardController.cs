using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuporteIA.Data;

namespace SuporteIA.Controllers
{
    [Authorize(Roles = "Tecnico,Administrador")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _ctx;
        public DashboardController(AppDbContext ctx) { _ctx = ctx; }

        public async Task<IActionResult> Index()
        {
            var porCategoria = await _ctx.Chamados
                .GroupBy(c => c.Categoria ?? "Sem Categoria")
                .Select(g => new { label = g.Key, value = g.Count() })
                .ToListAsync();

            var porStatus = await _ctx.Chamados
                .GroupBy(c => c.Status ?? "Aberto")
                .Select(g => new { label = g.Key, value = g.Count() })
                .ToListAsync();

            var porPrioridade = await _ctx.Chamados
                .GroupBy(c => c.Prioridade ?? "Normal")
                .Select(g => new { label = g.Key, value = g.Count() })
                .ToListAsync();

            ViewBag.PorCategoria = porCategoria;
            ViewBag.PorStatus = porStatus;
            ViewBag.PorPrioridade = porPrioridade;
            return View();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuporteIA.Data;
using SuporteIA.Models;

namespace SuporteIA.Controllers
{
    [Authorize]
    public class HistoricoController : Controller
    {
        private readonly AppDbContext _context;

        public HistoricoController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int cdChamado)
        {
            var chamado = await _context.Chamados
                .Include(c => c.Historicos)
                .FirstOrDefaultAsync(c => c.CdChamado == cdChamado);

            if (chamado == null)
            {
                TempData["Erro"] = "Chamado não encontrado.";
                return RedirectToAction("Index", "Chamado");
            }

            ViewBag.Chamado = chamado;
            return View(chamado.Historicos.OrderByDescending(h => h.DataRegistro).ToList());
        }

        [HttpGet]
        public IActionResult Create(int cdChamado)
        {
            ViewBag.CdChamado = cdChamado;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int cdChamado, string mensagem, string? solucao)
        {
            var chamado = await _context.Chamados.FindAsync(cdChamado);
            if (chamado == null)
            {
                TempData["Erro"] = "Chamado não encontrado.";
                return RedirectToAction("Index", "Chamado");
            }

            var historico = new Historico
            {
                FkCdChamado = cdChamado,
                MensagemArquivada = mensagem,
                FkMensagemSolucao = solucao,
                FkNmStatus = chamado.Status ?? "Aberto",
                DataRegistro = DateTime.Now
            };

            _context.Historicos.Add(historico);
            await _context.SaveChangesAsync();

            TempData["Mensagem"] = "Histórico registrado com sucesso!";
            return RedirectToAction(nameof(Index), new { cdChamado });
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int cdHistorico, int cdChamado)
        {
            var historico = await _context.Historicos.FindAsync(cdHistorico);
            if (historico == null)
            {
                TempData["Erro"] = "Registro de histórico não encontrado.";
                return RedirectToAction(nameof(Index), new { cdChamado });
            }

            _context.Historicos.Remove(historico);
            await _context.SaveChangesAsync();

            TempData["Mensagem"] = "Registro de histórico removido.";
            return RedirectToAction(nameof(Index), new { cdChamado });
        }
    }
}

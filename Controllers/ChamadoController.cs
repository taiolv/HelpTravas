using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuporteIA.Data;
using SuporteIA.Models;
using SuporteIA.Services;
using System.Linq;

namespace SuporteIA.Controllers
{
    [Authorize]
    public class ChamadoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IIASuporteService _ia;

        public ChamadoController(AppDbContext context, IIASuporteService ia)
        {
            _context = context;
            _ia = ia;
        }

        public IActionResult Index()
        {
            var chamados = _context.Chamados.ToList();
            return View(chamados);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Chamado chamado)
        {
            if (ModelState.IsValid)
            {
                var sugestao = await _ia.GerarSugestaoAsync(chamado.NmProblema);
                chamado.SolucaoIA = sugestao ?? "A IA não conseguiu gerar uma resposta.";
                chamado.DataAbertura = DateTime.Now;

                _context.Chamados.Add(chamado);
                await _context.SaveChangesAsync();

                TempData["Mensagem"] = "Chamado criado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            return View(chamado);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var chamado = _context.Chamados.FirstOrDefault(c => c.CdChamado == id);
            if (chamado == null)
            {
                TempData["Erro"] = "Chamado não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(chamado);
        }

        [Authorize(Roles = "Tecnico,Administrador")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var chamado = _context.Chamados.FirstOrDefault(c => c.CdChamado == id);
            if (chamado == null)
            {
                TempData["Erro"] = "Chamado não encontrado.";
                return RedirectToAction(nameof(Index));
            }
            return View(chamado);
        }

        [Authorize(Roles = "Tecnico,Administrador")]
        [HttpPost]
        public IActionResult Edit(Chamado chamado)
        {
            if (ModelState.IsValid)
            {
                var original = _context.Chamados.FirstOrDefault(c => c.CdChamado == chamado.CdChamado);
                if (original == null)
                {
                    TempData["Erro"] = "Chamado não encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                original.Categoria = chamado.Categoria;
                original.Status = chamado.Status;
                original.Prioridade = chamado.Prioridade;
                original.SolucaoIA = chamado.SolucaoIA;

                _context.SaveChanges();

                TempData["Mensagem"] = "Chamado atualizado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            return View(chamado);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var chamado = _context.Chamados.FirstOrDefault(c => c.CdChamado == id);
            if (chamado == null)
            {
                TempData["Erro"] = "Chamado não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            _context.Chamados.Remove(chamado);
            _context.SaveChanges();

            TempData["Mensagem"] = "Chamado excluído com sucesso.";
            return RedirectToAction(nameof(Index));
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuporteIA.Data;
using SuporteIA.Models;
using SuporteIA.Services;

namespace SuporteIA.Controllers
{
    [Authorize]
    public class FaqController : Controller
    {
        private readonly AppDbContext _ctx;
        private readonly IIASuporteService _ia;

        public FaqController(AppDbContext ctx, IIASuporteService ia)
        {
            _ctx = ctx;
            _ia = ia;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            var faqs = _ctx.Faqs.OrderByDescending(f => f.CriadoEm).ToList();
            return View(faqs);
        }

        [Authorize(Roles = "Tecnico,Administrador")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Tecnico,Administrador")]
        [HttpPost]
        public async Task<IActionResult> Create(string pergunta)
        {
            if (string.IsNullOrWhiteSpace(pergunta))
            {
                TempData["Erro"] = "Digite uma pergunta antes de enviar.";
                return RedirectToAction(nameof(Create));
            }

            try
            {
                var resposta = await _ia.GerarSugestaoAsync(pergunta);

                var faq = new Faq
                {
                    Pergunta = pergunta.Trim(),
                    Resposta = resposta ?? "A IA não retornou uma resposta.",
                    Fonte = "IA",
                    CriadoEm = DateTime.Now
                };

                _ctx.Faqs.Add(faq);
                await _ctx.SaveChangesAsync();

                TempData["Mensagem"] = "FAQ criada com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao gerar resposta da IA: " + ex.Message;
                return RedirectToAction(nameof(Create));
            }
        }

        [Authorize(Roles = "Tecnico,Administrador")]
        [HttpGet("Faq/CreateIA")]
        public IActionResult CreateIA()
        {

            return View("Create");
        }

        [Authorize(Roles = "Tecnico,Administrador")]
        [HttpPost("Faq/CreateIA")]
        public async Task<IActionResult> CreateIA(string pergunta)
        {
            if (string.IsNullOrWhiteSpace(pergunta))
            {
                TempData["Erro"] = "Digite uma pergunta antes de enviar.";
                return RedirectToAction(nameof(CreateIA));
            }

            try
            {
                var resposta = await _ia.GerarSugestaoAsync(pergunta);

                var faq = new Faq
                {
                    Pergunta = pergunta.Trim(),
                    Resposta = resposta ?? "A IA não retornou uma resposta.",
                    Fonte = "IA",
                    CriadoEm = DateTime.Now
                };

                _ctx.Faqs.Add(faq);
                await _ctx.SaveChangesAsync();

                TempData["Mensagem"] = "FAQ criada com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao gerar resposta da IA: " + ex.Message;
                return RedirectToAction(nameof(CreateIA));
            }
        }

        [Authorize(Roles = "Tecnico,Administrador")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var faq = await _ctx.Faqs.FindAsync(id);
            if (faq == null)
            {
                TempData["Erro"] = "FAQ não encontrada.";
                return RedirectToAction(nameof(Index));
            }

            _ctx.Faqs.Remove(faq);
            await _ctx.SaveChangesAsync();

            TempData["Mensagem"] = "FAQ excluída com sucesso!";
            return RedirectToAction(nameof(Index));
        }
    }
}

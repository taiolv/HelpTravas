using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuporteIA.Data;
using SuporteIA.Models;

namespace SuporteIA.Controllers
{
    [Authorize]
    public class ContaController : Controller
    {
        private readonly UserManager<AppUser> _um;
        private readonly SignInManager<AppUser> _sm;
        private readonly AppDbContext _ctx;

        public ContaController(UserManager<AppUser> um, SignInManager<AppUser> sm, AppDbContext ctx)
        {
            _um = um;
            _sm = sm;
            _ctx = ctx;
        }

        [HttpGet]
        public async Task<IActionResult> Perfil()
        {
            var user = await _um.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Auth");

            return View(user);
        }

        [HttpGet]
        public IActionResult Privacidade()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirMeusDados()
        {
            var user = await _um.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Auth");

            
            var meusChamados = await _ctx.Chamados
                .Where(c => c.SolicitanteUserId == user.Id)
                .ToListAsync();

            foreach (var c in meusChamados)
            {
                c.SolicitanteUserId = null;
                c.SolicitanteNome = "Anonimizado";
            }

            await _ctx.SaveChangesAsync();

            var userRoles = await _um.GetRolesAsync(user);
            if (userRoles.Any())
                await _um.RemoveFromRolesAsync(user, userRoles);

            var logins = await _um.GetLoginsAsync(user);
            foreach (var login in logins)
                await _um.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);

           
            var claims = await _um.GetClaimsAsync(user);
            if (claims.Any())
                await _um.RemoveClaimsAsync(user, claims);

            
            await _sm.SignOutAsync();

            
            var result = await _um.DeleteAsync(user);

            if (result.Succeeded)
            {
                TempData["Msg"] = "Seus dados foram removidos com sucesso, conforme a LGPD.";
                return RedirectToAction("Login", "Auth");
            }

            TempData["Erro"] = "Erro ao excluir conta: " +
                string.Join(", ", result.Errors.Select(e => e.Description));

            return RedirectToAction("Perfil");
        }
    }
}

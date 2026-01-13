using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SuporteIA.Models;

namespace SuporteIA.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuarioController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsuarioController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = _userManager.Users.ToList();
            var lista = new List<(AppUser User, IList<string> Roles)>();

            foreach (var u in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(u);
                lista.Add((u, roles));
            }

            return View(lista);
        }

        [HttpPost]
        public async Task<IActionResult> AlterarCargo(string userId, string novoCargo)
        {
            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null)
            {
                TempData["Erro"] = "Usuário não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var cargosAtuais = await _userManager.GetRolesAsync(usuario);
            await _userManager.RemoveFromRolesAsync(usuario, cargosAtuais);
            await _userManager.AddToRoleAsync(usuario, novoCargo);

            TempData["Mensagem"] = $"Cargo do usuário {usuario.NomeCompleto} alterado para {novoCargo}.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Excluir(string userId)
        {
            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null)
            {
                TempData["Erro"] = "Usuário não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            if (User.Identity?.Name == usuario.Email)
            {
                TempData["Erro"] = "Você não pode excluir a própria conta.";
                return RedirectToAction(nameof(Index));
            }

            var roles = await _userManager.GetRolesAsync(usuario);
            if (roles.Any())
            {
                await _userManager.RemoveFromRolesAsync(usuario, roles);
            }

            var result = await _userManager.DeleteAsync(usuario);

            if (result.Succeeded)
                TempData["Mensagem"] = "Usuário excluído com sucesso!";
            else
                TempData["Erro"] = "Erro ao excluir usuário: " + string.Join(", ", result.Errors.Select(e => e.Description));

            return RedirectToAction(nameof(Index));
        }

    }
}

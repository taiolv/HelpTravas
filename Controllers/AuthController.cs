using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SuporteIA.Models;

namespace SuporteIA.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterVm());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVm model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true,
                NomeCompleto = model.NomeCompleto,
                Cargo = "Usuário",
                DataCadastro = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, model.Senha);

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync("Usuario"))
                    await _roleManager.CreateAsync(new IdentityRole("Usuario"));

                await _userManager.AddToRoleAsync(user, "Usuario");
                await _signInManager.SignInAsync(user, isPersistent: false);

                return RedirectToAction("Index", "Chamado");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginVm());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVm model)
        {
            if (!ModelState.IsValid)
                return View(model);


            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Usuário não encontrado.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName,
                model.Senha,
                false,
                false
            );

            if (result.Succeeded)
                return RedirectToAction("Index", "Chamado");

            ModelState.AddModelError(string.Empty, "Email ou senha inválidos.");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}

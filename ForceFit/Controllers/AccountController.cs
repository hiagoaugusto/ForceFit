using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace ForceFit.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // === LOGIN ===

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string senha)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, senha, false, false);
                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var role = roles.FirstOrDefault();

                    if (role == null)
                    {
                        ViewBag.Erro = "Usuário não tem um papel atribuído.";
                        return View();
                    }

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, email),
                        new Claim(ClaimTypes.Role, role)
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    // Redirecionamento baseado no tipo de usuário
                    if (role == "Aluno")
                        return RedirectToAction("Perfil", "Aluno");
                    else if (role == "Personal")
                        return RedirectToAction("Index", "Personais");

                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Erro = "Login inválido";
            return View();
        }

        // === REGISTRO ===

        [HttpGet]
        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(string email, string senha, string role)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha) || string.IsNullOrEmpty(role))
            {
                ViewBag.Erro = "Todos os campos são obrigatórios.";
                return View();
            }

            var user = new IdentityUser { UserName = email, Email = email };

            // Verificar se o usuário já existe
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                ViewBag.Erro = "O usuário já está registrado.";
                return View();
            }

            var result = await _userManager.CreateAsync(user, senha);

            if (result.Succeeded)
            {
                // Verificar se a role existe antes de atribuir
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    ViewBag.Erro = "A role especificada não existe.";
                    return View();
                }

                await _userManager.AddToRoleAsync(user, role); // Atribui a role ao usuário
                TempData["Sucesso"] = "Usuário registrado! Faça login.";
                return RedirectToAction("Login");
            }

            ViewBag.Erro = "Erro ao registrar o usuário: " + string.Join(", ", result.Errors.Select(e => e.Description));
            return View();
        }

        // === LOGOUT ===

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}

using Microsoft.AspNetCore.Identity;

namespace ForceFit.Services
{
    public class FakeAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public FakeAuthService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Valida o login com base no UserManager
        public async Task<bool> ValidateUserAsync(string email, string senha)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, senha, false, false);
                return result.Succeeded;
            }
            return false;
        }

        // Registra um novo usuário
        public async Task<bool> RegisterUserAsync(string email, string senha, string role)
        {
            var user = new IdentityUser { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, senha);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role); // Adiciona a role
                return true;
            }
            return false;
        }

        // Retorna a role do usuário
        public async Task<string?> GetUserRoleAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                return roles.FirstOrDefault();
            }
            return null;
        }
    }
}

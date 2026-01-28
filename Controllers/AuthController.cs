using ClientesApp.Data;
using ClientesApp.Helpers;
using ClientesApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ClientesApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly ClientesContext _context;
        private const string AUTH_SCHEME = CookieAuthenticationDefaults.AuthenticationScheme;

        public AuthController(ClientesContext context)
        {
            _context = context;
        }

        // =====================
        // LOGIN
        // =====================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string login, string senha, bool lembrar = false)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(senha))
            {
                ModelState.AddModelError(string.Empty, "Informe login/email e senha");
                return View();
            }

            login = login.Trim().ToLowerInvariant();
            senha = senha.Trim();

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u =>
                u.Ativo &&
                (
                    u.Email == login ||
                    u.Login == login
                )
            );

            if (usuario == null || !Criptografia.Verificar(senha, usuario.SenhaHash))
            {
                ModelState.AddModelError(string.Empty, "Usuário ou senha inválidos");
                ViewBag.Login = login;
                return View();
            }

            usuario.UltimoLogin = DateTime.Now;
            usuario.UltimoAcesso = DateTime.Now;

            await _context.SaveChangesAsync();

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
        new Claim(ClaimTypes.Name, usuario.Nome),
        new Claim(ClaimTypes.Email, usuario.Email),
        new Claim(ClaimTypes.Role, usuario.Role) 
    };

            var identity = new ClaimsIdentity(claims, AUTH_SCHEME);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignOutAsync(AUTH_SCHEME);

            await HttpContext.SignInAsync(
                AUTH_SCHEME,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = lembrar,
                    ExpiresUtc = lembrar
                        ? DateTime.UtcNow.AddDays(7)
                        : DateTime.UtcNow.AddHours(2)
                });

            return RedirectToAction("Index", "Home");
        }


        // =====================
        // LOGOUT
        // =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(AUTH_SCHEME);
            return RedirectToAction(nameof(Login));
        }
    }
}

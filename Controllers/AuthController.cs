using BCrypt.Net;
using ClientesApp.Data;
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
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string senha, bool lembrar = false)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                ModelState.AddModelError("", "Informe email e senha");
                return View();
            }

            email = email.Trim().ToLower();
            senha = senha.Trim();

            // Busca usuário ativo
            var user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email && u.Ativo);

            // Valida senha com BCrypt
            if (user == null || !BCrypt.Net.BCrypt.Verify(senha, user.SenhaHash))
            {
                ModelState.AddModelError("", "Usuário ou senha inválidos");
                return View();
            }

            // Atualiza UltimoLogin
            user.UltimoLogin = DateTime.Now;
            _context.Usuarios.Update(user);
            await _context.SaveChangesAsync();

            // Cria claims e autentica usuário
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, user.Nome),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(claims, AUTH_SCHEME);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                AUTH_SCHEME,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = lembrar,
                    ExpiresUtc = lembrar ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddHours(2)
                });

            return RedirectToAction("Index", "Home");
        }

        // =====================
        // LOGOUT
        // =====================
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(AUTH_SCHEME);
            return RedirectToAction("Login");
        }

        // =====================
        // REGISTER
        // =====================
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string nome, string? login, string email, string senha)
        {
            if (string.IsNullOrWhiteSpace(nome) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(senha))
            {
                ModelState.AddModelError("", "Preencha todos os campos");
                return View();
            }

            email = email.Trim().ToLower();
            login = login?.Trim(); // nullable
            senha = senha.Trim();

            // Verifica duplicidade de email
            if (await _context.Usuarios.AnyAsync(u => u.Email == email))
            {
                ModelState.AddModelError("", "E-mail já cadastrado.");
                return View();
            }

            // Cria usuário
            var user = new Usuario
            {
                Nome = nome,
                Login = login,
                Email = email,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(senha),
                Ativo = true
            };

            _context.Usuarios.Add(user);
            await _context.SaveChangesAsync();

            TempData["msg"] = "Usuário cadastrado com sucesso. Faça login.";
            return RedirectToAction("Login");
        }
    }
}

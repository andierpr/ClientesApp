using ClientesApp.Data;
using ClientesApp.Enums;
using ClientesApp.Helpers;
using ClientesApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClientesApp.Controllers
{
    [Authorize(Roles = "Administrador")] // 🔒 Apenas administradores
    public class UsuariosController : Controller
    {
        private readonly ClientesContext _context;

        public UsuariosController(ClientesContext context)
        {
            _context = context;
        }

        // =====================
        // LISTAGEM DE USUÁRIOS
        // =====================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Usuarios
                .AsNoTracking()
                .OrderBy(u => u.Nome)
                .ToListAsync();

            return View(usuarios);
        }

        // =====================
        // NOVO USUÁRIO - GET
        // =====================
        [HttpGet]
        public IActionResult Novo()
        {
            var model = new UsuarioEditar
            {
                Ativo = true,
                Role = UserRole.Usuario
            };

            return View(model);
        }

        // =====================
        // NOVO USUÁRIO - POST
        // =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Novo(UsuarioEditar model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var email = model.Email.Trim().ToLower();
            var login = model.Login?.Trim();

            // Valida duplicidade de e-mail
            if (await _context.Usuarios.AnyAsync(u => u.Email.ToLower() == email))
            {
                ModelState.AddModelError("Email", "E-mail já cadastrado.");
                return View(model);
            }

            // Valida duplicidade de login (se informado)
            if (!string.IsNullOrWhiteSpace(login) &&
                await _context.Usuarios.AnyAsync(u => u.Login != null && u.Login.ToLower() == login.ToLower()))
            {
                ModelState.AddModelError("Login", "Login já cadastrado.");
                return View(model);
            }

            // Valida senha obrigatória
            if (string.IsNullOrWhiteSpace(model.NovaSenha))
            {
                ModelState.AddModelError("NovaSenha", "Senha é obrigatória.");
                return View(model);
            }

            // Cria novo usuário
            var usuario = new Usuario
            {
                Nome = model.Nome.Trim(),
                Login = login,
                Email = email,
                Ativo = model.Ativo,
                Role = model.Role.ToString(),
                SenhaHash = Criptografia.GerarHash(model.NovaSenha),
                CriadoEm = DateTime.UtcNow
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            TempData["msg"] = "Usuário cadastrado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        // =====================
        // EDITAR USUÁRIO - GET
        // =====================
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            if (id <= 0) return BadRequest();

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            var model = new UsuarioEditar
            {
                IdUsuario = usuario.IdUsuario,
                Nome = usuario.Nome,
                Login = usuario.Login,
                Email = usuario.Email,
                Ativo = usuario.Ativo,
                Role = Enum.Parse<UserRole>(usuario.Role)
            };

            return View(model);
        }

        // =====================
        // EDITAR USUÁRIO - POST
        // =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(UsuarioEditar model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = await _context.Usuarios.FindAsync(model.IdUsuario);
            if (usuario == null) return NotFound();

            var email = model.Email.Trim().ToLower();
            var login = model.Login?.Trim();

            // Valida duplicidade de e-mail
            if (await _context.Usuarios.AnyAsync(u => u.IdUsuario != model.IdUsuario && u.Email.ToLower() == email))
            {
                ModelState.AddModelError("Email", "E-mail já cadastrado.");
                return View(model);
            }

            // Valida duplicidade de login (se informado)
            if (!string.IsNullOrWhiteSpace(login) &&
                await _context.Usuarios.AnyAsync(u => u.IdUsuario != model.IdUsuario && u.Login != null && u.Login.ToLower() == login.ToLower()))
            {
                ModelState.AddModelError("Login", "Login já cadastrado.");
                return View(model);
            }

            // Atualiza dados do usuário
            usuario.Nome = model.Nome.Trim();
            usuario.Login = login;
            usuario.Email = email;
            usuario.Ativo = model.Ativo;
            usuario.Role = model.Role.ToString();

            // Atualiza senha se informada
            if (!string.IsNullOrWhiteSpace(model.NovaSenha))
                usuario.SenhaHash = Criptografia.GerarHash(model.NovaSenha);

            await _context.SaveChangesAsync();

            TempData["msg"] = "Usuário atualizado com sucesso.";
            return RedirectToAction(nameof(Index));
        }
    }
}

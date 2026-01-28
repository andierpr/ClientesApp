using ClientesApp.Data;
using ClientesApp.Helpers;
using ClientesApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClientesApp.Controllers.Api
{
    [ApiController]
    [Route("api/usuarios")]

    public class UsuariosApiController : ControllerBase
    {
        private readonly ClientesContext _context;

        public UsuariosApiController(ClientesContext context)
        {
            _context = context;
        }

        // =====================
        // CRIAR USUÁRIO
        // POST /api/usuarios
        // =====================
        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] UsuarioCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var email = dto.Email.Trim().ToLower();
            var login = dto.Login?.Trim();

            if (await _context.Usuarios.AnyAsync(u => u.Email.ToLower() == email))
                return Conflict("E-mail já cadastrado.");

            if (!string.IsNullOrWhiteSpace(login) &&
                await _context.Usuarios.AnyAsync(u =>
                    u.Login != null && u.Login.ToLower() == login.ToLower()))
                return Conflict("Login já cadastrado.");

            var usuario = new Usuario
            {
                Nome = dto.Nome.Trim(),
                Login = login,
                Email = email,
                Ativo = dto.Ativo,
                SenhaHash = Criptografia.GerarHash(dto.Senha),
                CriadoEm = DateTime.Now
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ObterPorId), new { id = usuario.IdUsuario }, new
            {
                usuario.IdUsuario,
                usuario.Nome,
                usuario.Login,
                usuario.Email,
                usuario.Ativo
            });
        }

        // =====================
        // BUSCAR USUÁRIO (EDIÇÃO)
        // GET /api/usuarios/{id}
        // =====================
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var usuario = await _context.Usuarios
                .AsNoTracking()
                .Where(u => u.IdUsuario == id)
                .Select(u => new UsuarioEditar
                {
                    IdUsuario = u.IdUsuario,
                    Nome = u.Nome,
                    Login = u.Login,
                    Email = u.Email,
                    Ativo = u.Ativo
                })
                .FirstOrDefaultAsync();

            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }

        // =====================
        // ATUALIZAR DADOS
        // PUT /api/usuarios/{id}
        // =====================
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] UsuarioEditar dto)
        {
            if (id != dto.IdUsuario)
                return BadRequest("Id inválido.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            var email = dto.Email.Trim().ToLower();
            var login = dto.Login?.Trim();

            if (await _context.Usuarios.AnyAsync(u =>
                    u.IdUsuario != id &&
                    u.Email.ToLower() == email))
                return Conflict("E-mail já cadastrado.");

            if (!string.IsNullOrWhiteSpace(login) &&
                await _context.Usuarios.AnyAsync(u =>
                    u.IdUsuario != id &&
                    u.Login != null &&
                    u.Login.ToLower() == login.ToLower()))
                return Conflict("Login já cadastrado.");

            usuario.Nome = dto.Nome.Trim();
            usuario.Login = login;
            usuario.Email = email;
            usuario.Ativo = dto.Ativo;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =====================
        // ATUALIZAR SENHA
        // PUT /api/usuarios/{id}/senha
        // =====================
        [HttpPut("{id:int}/senha")]
        public async Task<IActionResult> AtualizarSenha(int id, [FromBody] AtualizarSenhaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            usuario.SenhaHash = Criptografia.GerarHash(dto.NovaSenha);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using ClientesApp.Data;
using ClientesApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace ClientesApp.Controllers
{
    [Authorize]
    public class ClientesController : Controller
    {
        private readonly ClientesContext _context;

        public ClientesController(ClientesContext context)
        {
            _context = context;
        }

        // =========================
        // LISTAGEM (com ou sem filtro)
        // =========================
        [HttpGet]
        public async Task<IActionResult> Index(string? busca)
        {
            var query = QueryClientes();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                busca = busca.Trim();
                query = query.Where(c =>
                    c.Nome.Contains(busca) ||
                    c.Email.Contains(busca));
            }

            ViewBag.Busca = busca;

            var clientes = await query
                .OrderBy(c => c.Nome)
                .ToListAsync();

            return View(clientes);
        }

        // =========================
        // DETALHES
        // =========================
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
                return BadRequest();

            var cliente = await ObterClientePorIdAsync(id.Value, asNoTracking: true);

            return cliente is null
                ? NotFound()
                : View(cliente);
        }

        // =========================
        // CREATE
        // =========================
        public async Task<IActionResult> Create()
        {
            await CarregarEstadosAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,Email,Telefone,IdEstado")] Cliente cliente)
        {
            if (!ModelState.IsValid)
            {
                await CarregarEstadosAsync(cliente.IdEstado);
                return View(cliente);
            }

            _context.Add(cliente);
            await _context.SaveChangesAsync();

            TempData["msg"] = "Cliente cadastrado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EDIT
        // =========================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
                return BadRequest();

            var cliente = await ObterClientePorIdAsync(id.Value);

            if (cliente is null)
                return NotFound();

            await CarregarEstadosAsync(cliente.IdEstado);
            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Email,Telefone,IdEstado")] Cliente cliente)
        {
            if (id != cliente.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                await CarregarEstadosAsync(cliente.IdEstado);
                return View(cliente);
            }

            try
            {
                _context.Update(cliente);
                await _context.SaveChangesAsync();

                TempData["msg"] = "Cliente editado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ClienteExistsAsync(cliente.Id))
                    return NotFound();

                throw;
            }
        }

        // =========================
        // DELETE
        // =========================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
                return BadRequest();

            var cliente = await ObterClientePorIdAsync(id.Value, asNoTracking: true);

            return cliente is null
                ? NotFound()
                : View(cliente);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await ObterClientePorIdAsync(id);

            if (cliente is null)
                return NotFound();

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            TempData["msg"] = "Cliente excluído com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // MÉTODOS AUXILIARES
        // =========================

        private IQueryable<Cliente> QueryClientes() =>
            _context.Clientes
                .Include(c => c.Estado)
                .AsNoTracking();

        /// <summary>
        /// Carrega:
        ///  - ViewBag.Estados (dropdown NomeEstado)
        ///  - ViewBag.EstadosJson (IdEstado + UF para JS)
        /// </summary>
        private async Task CarregarEstadosAsync(object? selecionado = null)
        {
            var estados = await _context.Estados
                .OrderBy(e => e.NomeEstado)
                .AsNoTracking()
                .ToListAsync();

            ViewBag.Estados = new SelectList(
                estados,
                "IdEstado",
                "NomeEstado",
                selecionado
            );

            ViewBag.EstadosJson = JsonSerializer.Serialize(
                estados.Select(e => new
                {
                    e.IdEstado,
                    e.UF
                })
            );
        }

        private async Task<Cliente?> ObterClientePorIdAsync(int id, bool asNoTracking = false)
        {
            var query = _context.Clientes
                .Include(c => c.Estado)
                .AsQueryable();

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(c => c.Id == id);
        }

        private Task<bool> ClienteExistsAsync(int id) =>
            _context.Clientes.AnyAsync(c => c.Id == id);
    }
}

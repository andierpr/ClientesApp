using Microsoft.AspNetCore.Mvc;
using ClientesApp.Data;
using ClientesApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

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

        // LISTAR
        public async Task<IActionResult> Index()
        {
            var clientes = await _context.Clientes
                .Include(c => c.Estado) // Inclui Estado e UF
                .AsNoTracking()
                .ToListAsync();

            return View(clientes);
        }

        // Carregar lista de Estados para dropdown
        private async Task CarregarEstadosAsync(object? selecionado = null)
        {
            ViewBag.Estados = new SelectList(
                await _context.Estados
                    .OrderBy(e => e.NomeEstado)
                    .AsNoTracking()
                    .ToListAsync(),
                "IdEstado",
                "NomeEstado",
                selecionado
            );
        }

        // DETALHES
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
                return BadRequest();

            var cliente = await ObterClientePorIdAsync(id.Value, asNoTracking: true);

            if (cliente is null)
                return NotFound();

            return View(cliente);
        }

        // CREATE GET
        public async Task<IActionResult> Create()
        {
            await CarregarEstadosAsync();
            return View();
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,Email,Telefone,IdEstado")] Cliente cliente)
        {
            if (!ModelState.IsValid)
            {
                await CarregarEstadosAsync(cliente.IdEstado);
                return View(cliente);
            }

            try
            {
                _context.Add(cliente);
                await _context.SaveChangesAsync();

                TempData["msg"] = "Cliente cadastrado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Erro ao salvar o cliente.");
                await CarregarEstadosAsync(cliente.IdEstado);
                return View(cliente);
            }
        }

        // EDIT GET
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



        // EDIT POST
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



        // LISTAR (com filtro por nome ou email)
        [HttpGet]
        public async Task<IActionResult> Index(string? busca)
        {
            var query = _context.Clientes
                .Include(c => c.Estado)
                .AsNoTracking()
                .AsQueryable();

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




        // DELETE GET
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
                return BadRequest();

            var cliente = await ObterClientePorIdAsync(id.Value, asNoTracking: true);

            if (cliente is null)
                return NotFound();

            return View(cliente);
        }

        // DELETE POST
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

        // MÉTODO PRIVADO: Obter Cliente por Id (incluindo Estado)
        private async Task<Cliente?> ObterClientePorIdAsync(int id, bool asNoTracking = false)
        {
            var query = _context.Clientes.Include(c => c.Estado).AsQueryable();

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(c => c.Id == id);
        }

        // MÉTODO PRIVADO: Verificar se cliente existe
        private Task<bool> ClienteExistsAsync(int id) =>
            _context.Clientes.AnyAsync(c => c.Id == id);
    }
}

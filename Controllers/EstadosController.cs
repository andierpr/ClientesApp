
using ClientesApp.Data;
using ClientesApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClientesApp.Controllers
{
    [Authorize]
    public class EstadosController : Controller
    {
        private readonly ClientesContext _context;

        public EstadosController(ClientesContext context)
        {
            _context = context;
        }

        // INDEX (LISTAR + FILTRAR)
        [HttpGet]
        public async Task<IActionResult> Index(string? busca)
        {
            IQueryable<Estado> query = _context.Estados;

            if (!string.IsNullOrWhiteSpace(busca))
            {
                busca = busca.Trim();

                query = query.Where(e =>
                    EF.Functions.Like(e.NomeEstado!, $"%{busca}%") ||
                    EF.Functions.Like(e.UF!, $"%{busca}%")
                );
            }

            ViewBag.Busca = busca;

            var estados = await query
                .AsNoTracking()
                .OrderBy(e => e.NomeEstado)
                .ToListAsync();

            return View(estados);
        }

        // DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
                return BadRequest();

            var estado = await _context.Estados
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.IdEstado == id.Value);

            if (estado is null)
                return NotFound();

            return View(estado);
        }

        // CREATE GET
        public IActionResult Create() => View();

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UF,NomeEstado")] Estado estado)
        {
            if (!ModelState.IsValid)
                return View(estado);

            estado.UF = estado.UF?.Trim().ToUpper() ?? string.Empty;
            estado.NomeEstado = estado.NomeEstado?.Trim() ?? string.Empty;

            _context.Estados.Add(estado);
            await _context.SaveChangesAsync();

            TempData["msg"] = "Estado cadastrado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // EDIT GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
                return BadRequest();

            var estado = await _context.Estados.FindAsync(id.Value);

            if (estado is null)
                return NotFound();

            return View(estado);
        }

        // EDIT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEstado,UF,NomeEstado")] Estado estado)
        {
            if (id != estado.IdEstado)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(estado);

            try
            {
                estado.UF = estado.UF?.Trim().ToUpper() ?? string.Empty;
                estado.NomeEstado = estado.NomeEstado?.Trim() ?? string.Empty;

                _context.Update(estado);
                await _context.SaveChangesAsync();

                TempData["msg"] = "Estado editado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                bool exists = await EstadoExistsAsync(estado.IdEstado);

                if (!exists)
                    return NotFound();

                throw;
            }
        }

        // DELETE GET
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
                return BadRequest();

            var estado = await _context.Estados
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.IdEstado == id.Value);

            if (estado is null)
                return NotFound();

            return View(estado);
        }

        // DELETE POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var estado = await _context.Estados.FindAsync(id);

            if (estado is null)
                return NotFound();

            bool temClientes = await _context.Clientes
                .AnyAsync(c => c.IdEstado == id);

            if (temClientes)
            {
                TempData["msg"] = "Não é possível excluir o estado porque ele está vinculado a clientes.";
                return RedirectToAction(nameof(Index));
            }

            _context.Estados.Remove(estado);
            await _context.SaveChangesAsync();

            TempData["msg"] = "Estado excluído com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        private Task<bool> EstadoExistsAsync(int id)
        {
            return _context.Estados.AnyAsync(e => e.IdEstado == id);
        }
    }
}


//using Microsoft.AspNetCore.Mvc;
//using ClientesApp.Data;
//using ClientesApp.Models;
//using Microsoft.EntityFrameworkCore;

//namespace ClientesApp.Controllers
//{
//    public class EstadosController : Controller
//    {
//        private readonly ClientesContext _context;

//        public EstadosController(ClientesContext context)
//        {
//            _context = context;
//        }

//        // INDEX (LISTAR + FILTRAR)
//        [HttpGet]
//        public async Task<IActionResult> Index(string? busca)
//        {
//            var query = _context.Estados.AsQueryable();

//            if (!string.IsNullOrWhiteSpace(busca))
//            {
//                busca = busca.Trim();

//                query = query.Where(e =>
//                    EF.Functions.Like(e.NomeEstado, $"%{busca}%") ||
//                    EF.Functions.Like(e.UF, $"%{busca}%")
//                );
//            }

//            ViewBag.Busca = busca;

//            var estados = await query
//                .AsNoTracking()
//                .OrderBy(e => e.NomeEstado)
//                .ToListAsync();

//            return View(estados);
//        }

//        // DETAILS
//        public async Task<IActionResult> Details(int? id)
//        {
//            if (id is null)
//                return BadRequest();

//            var estado = await _context.Estados
//                .AsNoTracking()
//                .FirstOrDefaultAsync(e => e.IdEstado == id);

//            if (estado is null)
//                return NotFound();

//            return View(estado);
//        }

//        // CREATE GET
//        public IActionResult Create() => View();

//        // CREATE POST
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create([Bind("UF,NomeEstado")] Estado estado)
//        {
//            if (!ModelState.IsValid)
//                return View(estado);

//            estado.UF = estado.UF?.Trim().ToUpper();

//            _context.Estados.Add(estado);
//            await _context.SaveChangesAsync();

//            TempData["msg"] = "Estado cadastrado com sucesso!";
//            return RedirectToAction(nameof(Index));
//        }

//        // EDIT GET
//        public async Task<IActionResult> Edit(int? id)
//        {
//            if (id is null)
//                return BadRequest();

//            var estado = await _context.Estados.FindAsync(id);

//            if (estado is null)
//                return NotFound();

//            return View(estado);
//        }

//        // EDIT POST
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, [Bind("IdEstado,UF,NomeEstado")] Estado estado)
//        {
//            if (id != estado.IdEstado)
//                return BadRequest();

//            if (!ModelState.IsValid)
//                return View(estado);

//            try
//            {
//                estado.UF = estado.UF?.Trim().ToUpper();

//                _context.Update(estado);
//                await _context.SaveChangesAsync();

//                TempData["msg"] = "Estado editado com sucesso!";
//                return RedirectToAction(nameof(Index));
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!await EstadoExistsAsync(estado.IdEstado))
//                    return NotFound();

//                throw;
//            }
//        }

//        // DELETE GET
//        public async Task<IActionResult> Delete(int? id)
//        {
//            if (id is null)
//                return BadRequest();

//            var estado = await _context.Estados
//                .AsNoTracking()
//                .FirstOrDefaultAsync(e => e.IdEstado == id);

//            if (estado is null)
//                return NotFound();

//            return View(estado);
//        }

//        // DELETE POST
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            var estado = await _context.Estados.FindAsync(id);

//            if (estado is null)
//                return NotFound();

//            var temClientes = await _context.Clientes
//                .AnyAsync(c => c.IdEstado == id);

//            if (temClientes)
//            {
//                TempData["msg"] = "Não é possível excluir o estado porque ele está vinculado a clientes.";
//                return RedirectToAction(nameof(Index));
//            }

//            _context.Estados.Remove(estado);
//            await _context.SaveChangesAsync();

//            TempData["msg"] = "Estado excluído com sucesso!";
//            return RedirectToAction(nameof(Index));
//        }


//        private Task<bool> EstadoExistsAsync(int id) =>
//            _context.Estados.AnyAsync(e => e.IdEstado == id);
//    }
//}

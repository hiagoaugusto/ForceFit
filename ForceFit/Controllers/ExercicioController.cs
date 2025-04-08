using ForceFit.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ForceFit.Controllers
{
    [Authorize(Roles = "Personal")]
    public class ExerciciosController : Controller
    {
        private readonly Context _context;

        public ExerciciosController(Context context)
        {
            _context = context;
        }

        // GET: Exercicios
        public async Task<IActionResult> Index()
        {
            return View(await _context.Exercicios.ToListAsync());
        }

        // GET: Exercicios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var exercicio = await _context.Exercicios
                .Include(e => e.Treinos)
                .FirstOrDefaultAsync(m => m.ExercicioID == id);

            if (exercicio == null) return NotFound();

            return View(exercicio);
        }

        // GET: Exercicios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Exercicios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Exercicio exercicio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(exercicio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(exercicio);
        }

        // GET: Exercicios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var exercicio = await _context.Exercicios.FindAsync(id);
            if (exercicio == null) return NotFound();

            return View(exercicio);
        }

        // POST: Exercicios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExercicioID,NomeExercicio")] Exercicio exercicio)
        {
            if (id != exercicio.ExercicioID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exercicio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Exercicios.Any(e => e.ExercicioID == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(exercicio);
        }

        // GET: Exercicios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var exercicio = await _context.Exercicios
                .FirstOrDefaultAsync(m => m.ExercicioID == id);

            if (exercicio == null) return NotFound();

            return View(exercicio);
        }

        // POST: Exercicios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exercicio = await _context.Exercicios.FindAsync(id);
            if (exercicio != null)
            {
                _context.Exercicios.Remove(exercicio);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

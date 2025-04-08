using ForceFit.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ForceFit.Controllers
{
    [Authorize(Roles = "Personal")]
    public class TreinosController : Controller
    {
        private readonly Context _context;

        public TreinosController(Context context)
        {
            _context = context;
        }

        // GET: Treinos
        public async Task<IActionResult> Index()
        {
            var treinos = await _context.Treinos
                .Include(t => t.Personal)
                .Include(t => t.Aluno)
                .Include(t => t.Exercicios)
                .ToListAsync();
            return View(treinos);
        }

        // GET: Treinos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var treino = await _context.Treinos
                .Include(t => t.Personal)
                .Include(t => t.Aluno)
                .Include(t => t.Exercicios)
                .FirstOrDefaultAsync(t => t.TreinoID == id);

            if (treino == null) return NotFound();

            return View(treino);
        }

        // GET: Treinos/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Exercicios = await _context.Exercicios.ToListAsync();
            ViewBag.Personais = await _context.Personais.ToListAsync();
            ViewBag.Alunos = await _context.Alunos.ToListAsync();
            return View();
        }

        // POST: Treinos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string nomeTreino, int personalId, int alunoId, List<int> exercicioIds)
        {
            if (exercicioIds.Count < 4)
            {
                ModelState.AddModelError("", "O treino deve conter no mínimo 4 exercícios.");
                ViewBag.Exercicios = await _context.Exercicios.ToListAsync();
                ViewBag.Personais = await _context.Personais.ToListAsync();
                ViewBag.Alunos = await _context.Alunos.ToListAsync();
                return View();
            }

            var treino = new Treino
            {
                NomeTreino = nomeTreino,
                PersonalID = personalId,
                AlunoID = alunoId,
                Exercicios = await _context.Exercicios.Where(e => exercicioIds.Contains(e.ExercicioID)).ToListAsync()
            };

            _context.Treinos.Add(treino);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Treinos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var treino = await _context.Treinos
                .Include(t => t.Exercicios)
                .FirstOrDefaultAsync(t => t.TreinoID == id);

            if (treino == null) return NotFound();

            ViewBag.Exercicios = await _context.Exercicios.ToListAsync();
            ViewBag.Alunos = await _context.Alunos.ToListAsync();
            ViewBag.Personais = await _context.Personais.ToListAsync();
            return View(treino);
        }

        // POST: Treinos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string nomeTreino, int personalId, int alunoId, List<int> exercicioIds)
        {
            var treino = await _context.Treinos.Include(t => t.Exercicios).FirstOrDefaultAsync(t => t.TreinoID == id);

            if (treino == null) return NotFound();

            if (exercicioIds.Count < 4)
            {
                ModelState.AddModelError("", "O treino deve conter no mínimo 4 exercícios.");
                ViewBag.Exercicios = await _context.Exercicios.ToListAsync();
                ViewBag.Alunos = await _context.Alunos.ToListAsync();
                ViewBag.Personais = await _context.Personais.ToListAsync();
                return View(treino);
            }

            treino.NomeTreino = nomeTreino;
            treino.PersonalID = personalId;
            treino.AlunoID = alunoId;
            treino.Exercicios = await _context.Exercicios.Where(e => exercicioIds.Contains(e.ExercicioID)).ToListAsync();

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Treinos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var treino = await _context.Treinos
                .Include(t => t.Personal)
                .Include(t => t.Aluno)
                .FirstOrDefaultAsync(t => t.TreinoID == id);

            if (treino == null) return NotFound();

            return View(treino);
        }

        // POST: Treinos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var treino = await _context.Treinos.FindAsync(id);
            if (treino != null)
            {
                _context.Treinos.Remove(treino);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

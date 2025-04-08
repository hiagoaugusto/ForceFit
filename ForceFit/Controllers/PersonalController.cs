using ForceFit.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Logging; // Adicione este using

namespace ForceFit.Controllers
{
    [Authorize(Roles = "Personal")]
    public class PersonaisController : Controller
    {
        private readonly Context _context;
        private readonly ILogger<PersonaisController> _logger; // Adicione o logger

        public PersonaisController(Context context, ILogger<PersonaisController> logger) // Injete o logger
        {
            _context = context;
            _logger = logger;
        }

        // GET: Personais
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var personal = await _context.Personais
                .Where(p => p.IdentityUserId == userId)
                .ToListAsync();

            return View(personal);
        }

        // GET: Personais/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var personal = await _context.Personais
                .Include(p => p.Treinos)
                .ThenInclude(t => t.Exercicios)
                .FirstOrDefaultAsync(p => p.PersonalID == id);

            if (personal == null) return NotFound();

            return View(personal);
        }

        // GET: Personais/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Personais/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PersonalID, NomePersonal, Especialidade, IdentityUserId")] Personal personal) // Corrigido "PersonaID" para "PersonalID"
        {
            if (ModelState.IsValid)
            {
                personal.IdentityUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation($"Usuário '{User.Identity?.Name}' tentando criar Personal com IdentityUserId: '{personal.IdentityUserId}'"); // Adicionado log

                try
                {
                    _context.Add(personal);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Personal '{personal.NomePersonal}' criado com sucesso (ID: {personal.PersonalID}).");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, $"Erro ao salvar Personal '{personal.NomePersonal}' no banco de dados: {ex.InnerException?.Message}");
                    ModelState.AddModelError("", "Ocorreu um erro ao salvar o Personal. Por favor, tente novamente.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Erro inesperado ao criar Personal '{personal.NomePersonal}': {ex.Message}");
                    ModelState.AddModelError("", "Ocorreu um erro inesperado. Por favor, tente novamente.");
                }
            }
            else
            {
                _logger.LogWarning("ModelState inválido ao criar Personal. Erros:");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning($" - {error.ErrorMessage}");
                    ModelState.AddModelError("", error.ErrorMessage); // Adiciona erros de validação ao ModelState para exibição na View
                }
            }
            return View(personal);
        }

        // GET: Personais/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var personal = await _context.Personais.FindAsync(id);
            if (personal == null) return NotFound();

            return View(personal);
        }

        // POST: Personais/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PersonalID,NomePersonal,Especialidade")] Personal personal) // Adicionado Especialidade ao Bind do Edit
        {
            if (id != personal.PersonalID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    personal.IdentityUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Mantendo o IdentityUserId
                    _context.Update(personal);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Personais.Any(e => e.PersonalID == id)) return NotFound();
                    else throw;
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, $"Erro ao editar Personal '{personal.NomePersonal}' (ID: {id}): {ex.InnerException?.Message}");
                    ModelState.AddModelError("", "Ocorreu um erro ao salvar as alterações.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Erro inesperado ao editar Personal '{personal.NomePersonal}' (ID: {id}): {ex.Message}");
                    ModelState.AddModelError("", "Ocorreu um erro inesperado.");
                }
                return RedirectToAction(nameof(Index));
            }
            return View(personal);
        }

        // GET: Personais/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var personal = await _context.Personais.FindAsync(id);
            if (personal == null) return NotFound();

            return View(personal);
        }

        // POST: Personais/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var personal = await _context.Personais.FindAsync(id);
            if (personal != null)
            {
                _context.Personais.Remove(personal);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Personais/CreateTreino
        public async Task<IActionResult> CreateTreino(int personalId)
        {
            ViewBag.Exercicios = await _context.Exercicios.ToListAsync();
            ViewBag.Alunos = await _context.Alunos.ToListAsync();
            ViewBag.PersonalId = personalId;
            return View();
        }

        // POST: Personais/CreateTreino
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTreino(string nomeTreino, int personalId, int alunoId, List<int> exercicioIds)
        {
            if (exercicioIds == null || exercicioIds.Count < 1) // Alterado para >= 1 para teste, ajuste conforme necessário
            {
                ModelState.AddModelError("", "O treino deve conter pelo menos 1 exercício.");
                ViewBag.Exercicios = await _context.Exercicios.ToListAsync();
                ViewBag.Alunos = await _context.Alunos.ToListAsync();
                ViewBag.PersonalId = personalId;
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

            return RedirectToAction("Details", new { id = personalId });
        }
    }
}
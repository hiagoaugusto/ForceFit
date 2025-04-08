using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ForceFit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Authentication;

namespace ForceFit.Controllers
{
    [Authorize(Roles = "Aluno, Personal")]
    public class AlunoController : Controller
    { 
        public readonly Context _context;
        public readonly UserManager<IdentityUser> _userManager;

        public AlunoController(Context context,UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }



        // GET: ALUNO CREATE
        [AllowAnonymous]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [Authorize(Roles = "Aluno")]
        public async Task<IActionResult> Create(Aluno aluno)
        {
            if (ModelState.IsValid)
            {

                var userId = _userManager.GetUserId(User);
                aluno.IdentityUserId = userId;
                _context.Add(aluno);
                await _context.SaveChangesAsync();
                return View(aluno);
            }
            
            return RedirectToAction(nameof(Perfil));
        }
        //GET ALUNO PERFIL
        public async Task<IActionResult> Perfil()
        {
            var userId= _userManager.GetUserId(User);
            var aluno = await _context.Alunos.Include(a => a.Personal).FirstOrDefaultAsync(a => a.IdentityUserId == userId);
            if(aluno == null) return NotFound();
            return View(aluno);
        }
        //GET ALUNO DETAILS
        public async Task<IActionResult> Details()
        {
            var userId = _userManager.GetUserId(User);
            var aluno = await _context.Alunos.Include(a => a.Personal).FirstOrDefaultAsync(a => a.IdentityUserId == userId);
            if (aluno == null) return NotFound();
            return View(aluno);
        }
        //GET: ALUNO EDITAR
        public async Task<IActionResult> Editar()
        {
            var userId = _userManager.GetUserId(User);
            var aluno = await _context.Alunos.FirstOrDefaultAsync(a => a.IdentityUserId == userId);
            if (aluno == null) return NotFound();
            return View(aluno);
        }

        //post aluno editar
        [HttpPost]
        public async Task<IActionResult> Editar (Aluno model)
        {
            var userId = _userManager.GetUserId(User);
            var aluno = await _context.Alunos.FirstOrDefaultAsync(a =>a.IdentityUserId == userId);
            if (aluno == null || model.AlunoID != aluno.AlunoID) return NotFound();
            if (ModelState.IsValid)
            {
                aluno.NomeAluno = model.NomeAluno;
                aluno.Telefone = model.Telefone;
                aluno.Data_Nascimento = model.Data_Nascimento;
                _context.Update(aluno);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Perfil));
            }
            return View(model);
        }
        // get aluno delete
        public async Task<IActionResult> Delete()
        {
            var userId = _userManager.GetUserId(User);
            var aluno = await _context.Alunos.Include(a => a.Personal)
                .FirstOrDefaultAsync(a => a.IdentityUserId == userId);
            if (aluno == null) return NotFound();
            return View(aluno);
        }
        // POST: Aluno/DeleteConfirmed
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed()
        {
            var userId = _userManager.GetUserId(User);
            var aluno = await _context.Alunos.FirstOrDefaultAsync(a => a.IdentityUserId == userId);
            if (aluno == null) return NotFound();

            _context.Alunos.Remove(aluno);
            await _context.SaveChangesAsync();

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Treinos()
        {
            var userId = _userManager.GetUserId(User);
            var aluno = await _context.Alunos.Include(a => a.Treinos)
                .ThenInclude(t => t.Exercicios)
                .FirstOrDefaultAsync(a => a.IdentityUserId == userId);

            if (aluno == null) return NotFound();
            return View(aluno.Treinos);

        }
    }
}

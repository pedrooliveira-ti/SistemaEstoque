using Microsoft.AspNetCore.Mvc;
using ControleEstoqueRoupas.Models;
using ControleEstoqueRoupas.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ControleEstoqueRoupas.Controllers
{
    public class AuthController : Controller
    {
        private readonly EstoqueContext _context;

        public AuthController(EstoqueContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.Ativo);

                if (usuario != null && BCrypt.Net.BCrypt.Verify(model.Senha, usuario.SenhaHash))
                {
                    // Login bem-sucedido - implementar sessão aqui
                    return RedirectToAction("Index", "Produtos");
                }

                ModelState.AddModelError("", "Email ou senha inválidos");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(RegistroViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuarioExistente = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (usuarioExistente != null)
                {
                    ModelState.AddModelError("Email", "Este email já está em uso");
                    return View(model);
                }

                var usuario = new Usuario
                {
                    Nome = model.Nome,
                    Email = model.Email,
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword(model.Senha),
                    TipoUsuario = model.TipoUsuario,
                    NomeLoja = model.NomeLoja,
                    CNPJ = model.CNPJ,
                    DataCriacao = DateTime.Now,
                    Ativo = true
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login");
            }

            return View(model);
        }
    }
}
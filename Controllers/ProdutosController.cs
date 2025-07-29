using Microsoft.AspNetCore.Mvc;
using ControleEstoqueRoupas.Models;
using System.Linq;

namespace ControleEstoqueRoupas.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly EstoqueContext _context;

        public ProdutosController(EstoqueContext context)
        {
            _context = context;
        }

        // GET: Produtos
        public IActionResult Index()
        {
            var produtos = _context.Produtos.ToList();
            return View(produtos);
        }

        // POST: Produtos/Vender/5
        [HttpPost]
        public IActionResult Vender(int id)
        {
            var produto = _context.Produtos.Find(id);
            if (produto != null && produto.Quantidade > 0)
            {
                produto.Quantidade--;
                _context.SaveChanges();
                TempData["Mensagem"] = "Venda registrada!";
            }
            else
            {
                TempData["Mensagem"] = "Produto sem estoque!";
            }
            return RedirectToAction("Index");
        }

        // GET: Produtos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Produtos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Produto produto)
        {
            if (ModelState.IsValid)
            {
                _context.Produtos.Add(produto);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(produto);
        }
    }
}

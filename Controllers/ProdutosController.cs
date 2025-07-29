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

    // GET: Produtos/Edit/5
    public IActionResult Edit(int id)
    {
        var produto = _context.Produtos.Find(id);
        if (produto == null) return NotFound();
        return View(produto);
    }

    // POST: Produtos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Produto produto, IFormFile Imagem)
    {
        if (Imagem != null && Imagem.Length > 0)
        {
            var nomeImagem = Guid.NewGuid().ToString() + Path.GetExtension(Imagem.FileName);
            var caminho = Path.Combine("wwwroot/imagens", nomeImagem);
            using (var stream = new FileStream(caminho, FileMode.Create))
            {
                await Imagem.CopyToAsync(stream);
            }
            produto.CaminhoImagem = "/imagens/" + nomeImagem;
        }
        if (ModelState.IsValid)
        {
            _context.Update(produto);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        return View(produto);
    }

    // GET: Produtos/Delete/5
    public IActionResult Delete(int id)
    {
        var produto = _context.Produtos.Find(id);
        if (produto == null) return NotFound();
        return View(produto);
    }

    // POST: Produtos/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto != null)
        {
            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }

        // GET: Produtos
        public IActionResult Index(string search)
        {
            var produtos = from p in _context.Produtos select p;
            if (!string.IsNullOrEmpty(search))
            {
                produtos = produtos.Where(p => p.Nome.Contains(search));
            }
            return View(produtos.ToList());
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

        [HttpPost]
        public async Task<IActionResult> BaixarEstoque(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null) return NotFound();
            if (produto.Quantidade > 0)
                produto.Quantidade--;
            _context.Update(produto);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Produtos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Produtos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Produto produto, IFormFile Imagem)
        {
            if (Imagem != null && Imagem.Length > 0)
            {
                var nomeImagem = Guid.NewGuid().ToString() + Path.GetExtension(Imagem.FileName);
                var caminho = Path.Combine("wwwroot/imagens", nomeImagem);
                using (var stream = new FileStream(caminho, FileMode.Create))
                {
                    await Imagem.CopyToAsync(stream);
                }
                produto.CaminhoImagem = "/imagens/" + nomeImagem;
            }
            if (ModelState.IsValid)
            {
                _context.Produtos.Add(produto);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(produto);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ControleEstoqueRoupas.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace ControleEstoqueRoupas.Controllers
{

public class ProdutosController : Controller
{
    private readonly EstoqueContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProdutosController(EstoqueContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    // GET: Produtos
    public async Task<IActionResult> Index(string search)
    {
        var produtos = _context.Produtos.AsQueryable();
        
        if (!string.IsNullOrEmpty(search))
        {
            produtos = produtos.Where(p => p.Nome.Contains(search) || 
                                         p.CodigoCor.Contains(search) || 
                                         p.Tamanho.Contains(search));
            ViewData["search"] = search;
        }
        
        return View(await produtos.OrderBy(p => p.Nome).ToListAsync());
    }

    // GET: Produtos/Create
    public IActionResult Create()
    {
        return View(new Produto());
    }

    // POST: Produtos/Create
    [HttpPost]
    public async Task<IActionResult> Create(Produto produto, IFormFile? Imagem)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (Imagem != null && Imagem.Length > 0)
                {
                    var resultadoImagem = await SalvarImagem(Imagem);
                    if (resultadoImagem.sucesso)
                    {
                        produto.CaminhoImagem = resultadoImagem.caminho;
                    }
                    else
                    {
                        ModelState.AddModelError("Imagem", resultadoImagem.erro);
                        return View(produto);
                    }
                }

                _context.Produtos.Add(produto);
                await _context.SaveChangesAsync();
                TempData["Sucesso"] = "Produto cadastrado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Erro ao salvar produto: {ex.Message}");
        }
        
        return View(produto);
    }

    // GET: Produtos/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto == null)
        {
            TempData["Erro"] = "Produto não encontrado!";
            return RedirectToAction(nameof(Index));
        }
        return View(produto);
    }

    // POST: Produtos/Edit/5
    [HttpPost]
    public async Task<IActionResult> Edit(int id, Produto produto, IFormFile? Imagem)
    {
        if (id != produto.Id)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var produtoExistente = await _context.Produtos.FindAsync(id);
                if (produtoExistente == null)
                {
                    TempData["Erro"] = "Produto não encontrado!";
                    return RedirectToAction(nameof(Index));
                }

                // Atualizar propriedades
                produtoExistente.Nome = produto.Nome;
                produtoExistente.CodigoCor = produto.CodigoCor;
                produtoExistente.Quantidade = produto.Quantidade;
                produtoExistente.Tamanho = produto.Tamanho;

                // Processar nova imagem se fornecida
                if (Imagem != null && Imagem.Length > 0)
                {
                    var resultadoImagem = await SalvarImagem(Imagem);
                    if (resultadoImagem.sucesso)
                    {
                        // Remove imagem antiga se existir
                        if (!string.IsNullOrEmpty(produtoExistente.CaminhoImagem))
                        {
                            RemoverImagem(produtoExistente.CaminhoImagem);
                        }
                        produtoExistente.CaminhoImagem = resultadoImagem.caminho;
                    }
                    else
                    {
                        TempData["Erro"] = $"Erro ao salvar imagem: {resultadoImagem.erro}";
                        return View(produto);
                    }
                }

                _context.Update(produtoExistente);
                await _context.SaveChangesAsync();
                TempData["Sucesso"] = "Produto atualizado com sucesso!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProdutoExists(produto.Id))
                {
                    TempData["Erro"] = "Produto não encontrado!";
                }
                else
                {
                    TempData["Erro"] = "Erro de concorrência ao atualizar produto!";
                }
            }
            catch (Exception ex)
            {
                TempData["Erro"] = $"Erro ao atualizar produto: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
        return View(produto);
    }

    // GET: Produtos/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto == null)
        {
            TempData["Erro"] = "Produto não encontrado!";
            return RedirectToAction(nameof(Index));
        }
        return View(produto);
    }

    // POST: Produtos/Delete/5
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto != null)
            {
                // Remove imagem se existir
                if (!string.IsNullOrEmpty(produto.CaminhoImagem))
                {
                    RemoverImagem(produto.CaminhoImagem);
                }

                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();
                TempData["Sucesso"] = "Produto removido com sucesso!";
            }
            else
            {
                TempData["Erro"] = "Produto não encontrado!";
            }
        }
        catch (Exception ex)
        {
            TempData["Erro"] = $"Erro ao remover produto: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }

    // POST: Produtos/Vender/5
    [HttpPost]
    public async Task<IActionResult> Vender(int id)
    {
        try
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                TempData["Erro"] = "Produto não encontrado!";
                return RedirectToAction(nameof(Index));
            }

            if (produto.Quantidade <= 0)
            {
                TempData["Erro"] = "Produto sem estoque disponível!";
                return RedirectToAction(nameof(Index));
            }

            produto.Quantidade--;
            _context.Update(produto);
            await _context.SaveChangesAsync();
            
            TempData["Sucesso"] = $"Venda registrada! Estoque atual: {produto.Quantidade}";
        }
        catch (Exception ex)
        {
            TempData["Erro"] = $"Erro ao registrar venda: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }

    // POST: Produtos/AdicionarEstoque/5
    [HttpPost]
    public async Task<IActionResult> AdicionarEstoque(int id, int quantidade = 1)
    {
        try
        {
            if (quantidade <= 0)
            {
                TempData["Erro"] = "Quantidade deve ser maior que zero!";
                return RedirectToAction(nameof(Index));
            }

            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                TempData["Erro"] = "Produto não encontrado!";
                return RedirectToAction(nameof(Index));
            }

            produto.Quantidade += quantidade;
            _context.Update(produto);
            await _context.SaveChangesAsync();
            
            TempData["Sucesso"] = $"Adicionado {quantidade} unidade(s) ao estoque! Total: {produto.Quantidade}";
        }
        catch (Exception ex)
        {
            TempData["Erro"] = $"Erro ao adicionar estoque: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }

    // Métodos privados auxiliares
    private async Task<(bool sucesso, string caminho, string erro)> SalvarImagem(IFormFile imagem)
    {
        try
        {
            // Validações
            if (imagem == null || imagem.Length == 0)
                return (false, "", "Arquivo de imagem inválido");

            // Verificar tamanho (máx 5MB)
            if (imagem.Length > 5 * 1024 * 1024)
                return (false, "", "Imagem muito grande. Máximo 5MB");

            // Verificar extensão
            var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extensao = Path.GetExtension(imagem.FileName).ToLowerInvariant();
            if (!extensoesPermitidas.Contains(extensao))
                return (false, "", "Formato não suportado. Use: JPG, PNG, GIF ou WebP");

            // Gerar nome único
            var nomeImagem = Guid.NewGuid().ToString() + extensao;
            var diretorioImagens = Path.Combine(_webHostEnvironment.WebRootPath, "imagens");
            var caminhoCompleto = Path.Combine(diretorioImagens, nomeImagem);
            
            // Criar diretório se não existir
            Directory.CreateDirectory(diretorioImagens);
            
            // Salvar arquivo
            using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
            {
                await imagem.CopyToAsync(stream);
            }
            
            return (true, "/imagens/" + nomeImagem, "");
        }
        catch (Exception ex)
        {
            return (false, "", $"Erro ao salvar imagem: {ex.Message}");
        }
    }

    private void RemoverImagem(string caminhoImagem)
    {
        try
        {
            if (string.IsNullOrEmpty(caminhoImagem)) return;

            var caminhoCompleto = Path.Combine(_webHostEnvironment.WebRootPath, 
                                             caminhoImagem.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            
            if (System.IO.File.Exists(caminhoCompleto))
            {
                System.IO.File.Delete(caminhoCompleto);
            }
        }
        catch (Exception ex)
        {
            // Log do erro se necessário, mas não interrompe o fluxo
            System.Diagnostics.Debug.WriteLine($"Erro ao remover imagem: {ex.Message}");
        }
    }

    private bool ProdutoExists(int id)
    {
        return _context.Produtos.Any(e => e.Id == id);
    }
}

}








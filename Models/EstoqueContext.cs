using Microsoft.EntityFrameworkCore;

namespace ControleEstoqueRoupas.Models
{
    public class EstoqueContext : DbContext
    {
        public EstoqueContext(DbContextOptions<EstoqueContext> options) : base(options)
        {
        }

        public DbSet<Produto> Produtos { get; set; } = null!;
    }
}






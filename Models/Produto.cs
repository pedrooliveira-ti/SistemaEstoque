namespace ControleEstoqueRoupas.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string CodigoCor { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public string Tamanho { get; set; } = string.Empty;
        public string CaminhoFoto { get; set; } = string.Empty;
        public string? CaminhoImagem { get; set; }
    }
}



using System.ComponentModel.DataAnnotations;

namespace ControleEstoqueRoupas.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        
        [Required]
        public string Nome { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string SenhaHash { get; set; } = string.Empty;
        
        public string TipoUsuario { get; set; } = "Cliente";
        
        public string? NomeLoja { get; set; }
        
        public string? CNPJ { get; set; }
        
        public DateTime DataCriacao { get; set; }
        
        public bool Ativo { get; set; } = true;
    }
}
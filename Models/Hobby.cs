using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace test_crud_with_jwt.Models
{
    public class Hobby
    {
        public int Id { get; set; }
        
        [Required]
        [DisplayName("Nama Hobi")]
        public string Name { get; set; } = string.Empty;
        
        [DisplayName("Deskripsi")]
        public string? Description { get; set; }
        
        [DisplayName("Dibuat Pada")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        [DisplayName("Diperbarui Pada")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        
        // Foreign key
        public int UserId { get; set; }
        
        // Navigation property
        public virtual User User { get; set; } = null!;
    }
}
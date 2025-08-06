using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace test_crud_with_jwt.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [DisplayName("Nama")]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [DisplayName("Email")]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [DisplayName("Password")]
        public string PasswordHash { get; set; } = string.Empty;
        
        [DisplayName("Umur")]
        public int Age { get; set; }
        
        [DisplayName("Alamat")]
        public string? Address { get; set; }
        
        [DisplayName("Dibuat Pada")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        [DisplayName("Diperbarui Pada")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        
        // Navigation property for one-to-many relationship
        public virtual ICollection<Hobby> Hobbies { get; set; } = new List<Hobby>();
    }
}
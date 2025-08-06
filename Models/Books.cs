using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace test_crud_with_jwt.Models
{
    public class Books
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("Book Title")]
        public string BookTitle { get; set; }

        [DisplayName("Book Description")]
        public string BookDescription { get; set; }

        [Required]
        public string Author { get; set; }
    }
}

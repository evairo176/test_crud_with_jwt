using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using test_crud_with_jwt.DAL;
using test_crud_with_jwt.Models;

namespace test_crud_with_jwt.Pages.Book
{
    public class IndexModel : PageModel
    {
        private readonly MyAppDbContext _context;

        public IndexModel(MyAppDbContext context)
        {
            _context = context;
        }

        public IList<Books> Books { get; set; }

        public async Task  OnGetAsync()
        {
            Books = await _context.Books.ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);

            await _context.SaveChangesAsync();

            return Page();
        }
    }
}

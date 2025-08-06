using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using test_crud_with_jwt.DAL;
using test_crud_with_jwt.Models;

namespace test_crud_with_jwt.Pages.Book
{
    public class EditModel : PageModel
    {
        private readonly MyAppDbContext _context;

        public EditModel(MyAppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Books Books { get; set; } = default;
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if(id == null)
            {
                return RedirectToPage("./Index");
            }

            var book = await _context.Books.FirstOrDefaultAsync(book => book.Id == id);

            if (book == null)
            {
                return RedirectToPage("./Index");
            }

            Books = book;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Books.Update(Books);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}

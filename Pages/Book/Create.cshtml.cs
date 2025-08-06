using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using test_crud_with_jwt.DAL;
using test_crud_with_jwt.Models;

namespace test_crud_with_jwt.Pages.Book
{
    public class CreateModel : PageModel
    {

        private readonly MyAppDbContext _context;

        public CreateModel(MyAppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Books Books { get; set; } = default;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Books.Add(Books);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace test_crud_with_jwt.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Redirect to login if not authenticated, otherwise to users list
            if (Request.Cookies.ContainsKey("jwt"))
            {
                return RedirectToPage("/Users/Index");
            }
            return RedirectToPage("/Login");
        }
    }
}

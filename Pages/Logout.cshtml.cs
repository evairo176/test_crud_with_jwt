using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace test_crud_with_jwt.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            try
            {
                // Remove JWT cookie with matching options
                Response.Cookies.Delete("jwt", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = Request.IsHttps, // Match the login cookie settings
                    SameSite = SameSiteMode.Strict,
                    Path = "/"
                });

                // Also try to delete without options as fallback
                Response.Cookies.Delete("jwt");

                // Clear any session data
                HttpContext.Session.Clear();

                // Add success message
                TempData["SuccessMessage"] = "Anda telah berhasil logout.";

                Console.WriteLine("=== LOGOUT DEBUG ===");
                Console.WriteLine("User logged out successfully");
                Console.WriteLine("JWT cookie deleted");
                Console.WriteLine($"Request.IsHttps: {Request.IsHttps}");
                Console.WriteLine("====================");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logout error: {ex.Message}");
                TempData["ErrorMessage"] = "Terjadi kesalahan saat logout.";
            }
            
            return RedirectToPage("/Login");
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using test_crud_with_jwt.Models;
using test_crud_with_jwt.Services;

namespace test_crud_with_jwt.Pages
{
    public class LoginModel : PageModel
    {
        private readonly AuthService _authService;

        public LoginModel(AuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public Models.LoginModel LoginData { get; set; } = new();

        public IActionResult OnGet()
        {
            // Redirect if already authenticated
            if (Request.Cookies.ContainsKey("jwt"))
            {
                return RedirectToPage("/Users/Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var token = await _authService.LoginAsync(LoginData.Email, LoginData.Password);
            if (token == null)
            {
                ModelState.AddModelError(string.Empty, "Email atau password salah.");
                return Page();
            }

            // Set JWT token in cookie
            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps, // Only secure in HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Now.AddHours(24),
                Path = "/"
            });

            return RedirectToPage("/Users/Index");
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using test_crud_with_jwt.Services;

namespace test_crud_with_jwt.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly AuthService _authService;

        public RegisterModel(AuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        [Required]
        [Display(Name = "Nama")]
        public string Name { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [MinLength(6)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [Compare("Password", ErrorMessage = "Password dan konfirmasi password tidak sama.")]
        [Display(Name = "Konfirmasi Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [Range(1, 120, ErrorMessage = "Umur harus antara 1-120 tahun")]
        [Display(Name = "Umur")]
        public int Age { get; set; }

        [BindProperty]
        [Display(Name = "Alamat")]
        public string? Address { get; set; }

        public IActionResult OnGet()
        {
            // Redirect if already authenticated
            if (Request.Cookies.ContainsKey("jwt"))
            {
                return RedirectToPage("/Profile");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var success = await _authService.RegisterAsync(Email, Password, Name, Age, Address);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Email sudah terdaftar.");
                return Page();
            }

            TempData["SuccessMessage"] = "Registrasi berhasil! Silakan login.";
            return RedirectToPage("/Login");
        }
    }
}
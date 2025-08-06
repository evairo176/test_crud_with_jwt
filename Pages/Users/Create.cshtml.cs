using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using test_crud_with_jwt.Models;
using test_crud_with_jwt.Services;

namespace test_crud_with_jwt.Pages.Users
{
    public class CreateModel : BasePageModel
    {
        private readonly UserService _userService;

        public CreateModel(JwtService jwtService, UserService userService) : base(jwtService)
        {
            _userService = userService;
        }

        [BindProperty]
        public User User { get; set; } = new();

        [BindProperty]
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public List<string> HobbyNames { get; set; } = new();

        public IActionResult OnGet()
        {
            var authResult = RequireAuthentication();
            if (authResult != null) return authResult;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var authResult = RequireAuthentication();
            if (authResult != null) return authResult;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var success = await _userService.CreateUserAsync(User, Password, HobbyNames);
            if (success)
            {
                TempData["SuccessMessage"] = "User berhasil ditambahkan.";
                return RedirectToPage("Index");
            }

            ModelState.AddModelError(string.Empty, "Gagal menambahkan user. Email mungkin sudah digunakan.");
            return Page();
        }
    }
}
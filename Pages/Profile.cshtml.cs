using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using test_crud_with_jwt.Services;

namespace test_crud_with_jwt.Pages
{
    public class ProfileModel : BasePageModel
    {
        private readonly UserService _userService;
        private readonly AuthService _authService;

        public ProfileModel(JwtService jwtService, UserService userService, AuthService authService) : base(jwtService)
        {
            _userService = userService;
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
        [Range(1, 120, ErrorMessage = "Umur harus antara 1-120 tahun")]
        [Display(Name = "Umur")]
        public int Age { get; set; }

        [BindProperty]
        [Display(Name = "Alamat")]
        public string? Address { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int TotalHobbies { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var authResult = RequireAuthentication();
            if (authResult != null) return authResult;

            var userId = _authService.GetCurrentUserId(Request.Cookies["jwt"]);
            if (userId == null) return RedirectToPage("/Login");

            var user = await _userService.GetCurrentUserAsync(userId.Value);
            if (user == null) return RedirectToPage("/Login");

            Name = user.Name;
            Email = user.Email;
            Age = user.Age;
            Address = user.Address;
            CreatedAt = user.CreatedAt;
            UpdatedAt = user.UpdatedAt;
            TotalHobbies = user.Hobbies.Count;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var authResult = RequireAuthentication();
            if (authResult != null) return authResult;

            var userId = _authService.GetCurrentUserId(Request.Cookies["jwt"]);
            if (userId == null) return RedirectToPage("/Login");

            if (!ModelState.IsValid)
            {
                // Reload user data for display
                var user = await _userService.GetCurrentUserAsync(userId.Value);
                if (user != null)
                {
                    CreatedAt = user.CreatedAt;
                    UpdatedAt = user.UpdatedAt;
                    TotalHobbies = user.Hobbies.Count;
                }
                return Page();
            }

            var success = await _userService.UpdateUserProfileAsync(userId.Value, Name, Email, Age, Address);
            if (success)
            {
                TempData["SuccessMessage"] = "Profil berhasil diperbarui.";
            }
            else
            {
                TempData["ErrorMessage"] = "Gagal memperbarui profil. Email mungkin sudah digunakan.";
            }

            return RedirectToPage();
        }
    }
}
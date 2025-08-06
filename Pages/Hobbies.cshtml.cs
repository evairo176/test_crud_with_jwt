using Microsoft.AspNetCore.Mvc;
using test_crud_with_jwt.Models;
using test_crud_with_jwt.Services;

namespace test_crud_with_jwt.Pages
{
    public class HobbiesModel : BasePageModel
    {
        private readonly UserService _userService;
        private readonly AuthService _authService;

        public HobbiesModel(JwtService jwtService, UserService userService, AuthService authService) : base(jwtService)
        {
            _userService = userService;
            _authService = authService;
        }

        public List<Hobby> Hobbies { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public string SearchTerm { get; set; } = string.Empty;

        public string GetPageUrl(int page)
        {
            if (string.IsNullOrEmpty(SearchTerm))
            {
                return $"?page={page}";
            }
            return $"?page={page}&searchTerm={Uri.EscapeDataString(SearchTerm)}";
        }

        public async Task<IActionResult> OnGetAsync([FromQuery] int page = 1, [FromQuery] string searchTerm = "")
        {
            var authResult = RequireAuthentication();
            if (authResult != null) return authResult;

            var userId = _authService.GetCurrentUserId(Request.Cookies["jwt"]);
            if (userId == null) return RedirectToPage("/Login");

            // Validate page parameter
            if (page < 1) page = 1;

            CurrentPage = page;
            SearchTerm = searchTerm ?? string.Empty;

            var (hobbies, totalCount) = await _userService.GetUserHobbiesAsync(userId.Value, page, 10, SearchTerm);
            Hobbies = hobbies;
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling((double)totalCount / 10);

            // If requested page is beyond available pages, redirect to last page
            if (page > TotalPages && TotalPages > 0)
            {
                return RedirectToPage("Hobbies", new { page = TotalPages, searchTerm = SearchTerm });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAddHobbyAsync(string hobbyName, string? description)
        {
            var authResult = RequireAuthentication();
            if (authResult != null) return authResult;

            var userId = _authService.GetCurrentUserId(Request.Cookies["jwt"]);
            if (userId == null) return RedirectToPage("/Login");

            if (string.IsNullOrWhiteSpace(hobbyName))
            {
                TempData["ErrorMessage"] = "Nama hobi tidak boleh kosong.";
                return RedirectToPage();
            }

            var success = await _userService.AddHobbyAsync(userId.Value, hobbyName, description);
            if (success)
            {
                TempData["SuccessMessage"] = "Hobi berhasil ditambahkan.";
            }
            else
            {
                TempData["ErrorMessage"] = "Gagal menambahkan hobi.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditHobbyAsync(int hobbyId, string hobbyName, string? description)
        {
            var authResult = RequireAuthentication();
            if (authResult != null) return authResult;

            var userId = _authService.GetCurrentUserId(Request.Cookies["jwt"]);
            if (userId == null) return RedirectToPage("/Login");

            if (string.IsNullOrWhiteSpace(hobbyName))
            {
                TempData["ErrorMessage"] = "Nama hobi tidak boleh kosong.";
                return RedirectToPage();
            }

            var success = await _userService.UpdateHobbyAsync(userId.Value, hobbyId, hobbyName, description);
            if (success)
            {
                TempData["SuccessMessage"] = "Hobi berhasil diperbarui.";
            }
            else
            {
                TempData["ErrorMessage"] = "Gagal memperbarui hobi.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteHobbyAsync(int hobbyId)
        {
            var authResult = RequireAuthentication();
            if (authResult != null) return authResult;

            var userId = _authService.GetCurrentUserId(Request.Cookies["jwt"]);
            if (userId == null) return RedirectToPage("/Login");

            var success = await _userService.DeleteHobbyAsync(userId.Value, hobbyId);
            if (success)
            {
                TempData["SuccessMessage"] = "Hobi berhasil dihapus.";
            }
            else
            {
                TempData["ErrorMessage"] = "Gagal menghapus hobi.";
            }

            return RedirectToPage();
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using test_crud_with_jwt.Models;
using test_crud_with_jwt.Services;

namespace test_crud_with_jwt.Pages.Users
{
    public class IndexModel : BasePageModel
    {
        private readonly UserService _userService;

        public IndexModel(JwtService jwtService, UserService userService) : base(jwtService)
        {
            _userService = userService;
        }

        public List<User> Users { get; set; } = new();
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
            try
            {
                var authResult = RequireAuthentication();
                if (authResult != null) return authResult;

                // Add cache-busting headers
                Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                Response.Headers["Pragma"] = "no-cache";
                Response.Headers["Expires"] = "0";

                // Validate page parameter
                if (page < 1) page = 1;

                CurrentPage = page;
                SearchTerm = searchTerm ?? string.Empty;

                var (users, totalCount) = await _userService.GetAllUsersAsync(page, 10, SearchTerm);
                Users = users;
                TotalCount = totalCount;
                TotalPages = (int)Math.Ceiling((double)totalCount / 10);

                // If requested page is beyond available pages, redirect to last page
                if (page > TotalPages && TotalPages > 0)
                {
                    return RedirectToPage("Index", new { page = TotalPages, searchTerm = SearchTerm });
                }

                return Page();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Terjadi kesalahan: {ex.Message}";
                return RedirectToPage("Index");
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var authResult = RequireAuthentication();
            if (authResult != null) return authResult;

            var success = await _userService.DeleteUserAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "User berhasil dihapus.";
            }
            else
            {
                TempData["ErrorMessage"] = "Gagal menghapus user.";
            }

            return RedirectToPage();
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using test_crud_with_jwt.Services;

namespace test_crud_with_jwt.Pages
{
    public class BasePageModel : PageModel
    {
        protected readonly JwtService _jwtService;

        public BasePageModel(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        protected bool IsAuthenticated()
        {
            var token = Request.Cookies["jwt"];
            if (string.IsNullOrEmpty(token))
                return false;

            var principal = _jwtService.ValidateToken(token);
            return principal != null;
        }

        protected IActionResult RequireAuthentication()
        {
            if (!IsAuthenticated())
            {
                return RedirectToPage("/Login");
            }
            return null!;
        }
    }
}
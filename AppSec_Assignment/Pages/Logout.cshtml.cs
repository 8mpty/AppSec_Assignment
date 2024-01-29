using AppSec_Assignment.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AppSec_Assignment.Pages
{
    public class LogoutModel : PageModel
    {
		private readonly SignInManager<NewUser> signInManager;
		public LogoutModel(SignInManager<NewUser> signInManager)
		{
			this.signInManager = signInManager;
		}
		public void OnGet() {
		}

		public async Task<IActionResult> OnPostLogoutAsync()
		{
			await signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            Response.Cookies.Delete("MyCookieAuth");
            Response.Cookies.Delete("AuthToken");
            Response.Cookies.Delete(".AspNetCore.Session");
            return RedirectToPage("Login");
		}
		public async Task<IActionResult> OnPostDontLogoutAsync()
		{
			return RedirectToPage("Index");
		}
	}
}

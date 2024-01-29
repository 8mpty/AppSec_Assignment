using AppSec_Assignment.Model;
using AppSec_Assignment.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using Nancy.Json;

namespace AppSec_Assignment.Pages
{
    public class LoginModel : PageModel
    {
		public class CustomObject
		{
			public string success { get; set; }
			public List<String> ErrorMsg { get; set; }
		}

        [BindProperty]
		public Login LModel { get; set; }
		private readonly SignInManager<NewUser> signInManager;
		private readonly UserManager<NewUser> userManager;
		public LoginModel(SignInManager<NewUser> signInManager, UserManager<NewUser> userManager)
		{
			this.signInManager = signInManager;
			this.userManager = userManager;
		}
		public void OnGet() {}
		public async Task<bool> ValidateCaptcha()
		{
			bool result = true;

			string captchaResponse = Request.Form["g-recaptcha-response"];
			var secretKey = "6LcTFl8pAAAAAJoSvT86aqW2_Dd7vUCY_5nXA36J";
			var url = $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={captchaResponse}";

			try
			{
				using (HttpClient client = new HttpClient())
				{
					HttpResponseMessage response = await client.GetAsync(url);

					if (response.IsSuccessStatusCode)
					{
						string jsonResponse = await response.Content.ReadAsStringAsync();

						CustomObject jsonObject = JsonConvert.DeserializeObject<CustomObject>(jsonResponse);

						result = Convert.ToBoolean(jsonObject.success);
					}
					else
					{
						result = false;
					}
				}

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}

		[ValidateAntiForgeryToken]
		public async Task<IActionResult> OnPostAsync()
		{
            var sessionAuthToken = HttpContext.Session.GetString("AuthToken");
            var cookieAuthToken = Request.Cookies["AuthToken"];

            if (sessionAuthToken == null || !sessionAuthToken.Equals(cookieAuthToken))
            {
                await signInManager.SignOutAsync();
                HttpContext.Session.Clear();

                Response.Cookies.Delete("MyCookieAuth");
                Response.Cookies.Delete("AuthToken");
                Response.Cookies.Delete(".AspNetCore.Session");
                Response.Redirect("/Login");
            }
            var authToken = Guid.NewGuid().ToString();
			HttpContext.Session.SetString("AuthToken", authToken);
			Response.Cookies.Append("AuthToken", authToken);

			if (ModelState.IsValid)
			{
				bool isCaptchaValid = await ValidateCaptcha();

				if (!isCaptchaValid)
				{
					ModelState.AddModelError("", "reCAPTCHA validation failed. Please try again.");
					return Page();
				}
                var user = await userManager.FindByEmailAsync(LModel.Email);

                var alreadyLoggedIn = HttpContext.Session.GetString("AlreadyLoggedIn");
                if (alreadyLoggedIn != null && alreadyLoggedIn.Equals("true"))
                {
                    await signInManager.SignOutAsync();
                    HttpContext.Session.Clear();
                    Response.Cookies.Delete("MyCookieAuth");
                    Response.Cookies.Delete("AuthToken");
                    Response.Cookies.Delete(".AspNetCore.Session");
                    return RedirectToPage("/errors/403");
                }

                var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password, LModel.RememberMe, false);
				if (identityResult.Succeeded)
				{
					user.Logs += 1;
                    var claims = new List<Claim>
					{
						new Claim(ClaimTypes.Name, "c@c.com"),
						new Claim(ClaimTypes.Email, "c@c.com")
					};
					var i = new ClaimsIdentity(claims, "MyCookieAuth");
					ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(i);
					await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);
					await userManager.UpdateAsync(user);
                    HttpContext.Session.SetString("AlreadyLoggedIn", "true");
                    return RedirectToPage("Index");
				}
				else
				{
					ModelState.AddModelError("", "Username or Password incorrect");
				}
			}
			return Page();
		}
	}
}
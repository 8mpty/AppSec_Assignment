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

            if (ModelState.IsValid)
			{
				bool isCaptchaValid = await ValidateCaptcha();

				if (!isCaptchaValid)
				{
					ModelState.AddModelError("", "reCAPTCHA validation failed. Please try again.");
					return Page();
				}
                var user = await userManager.FindByEmailAsync(LModel.Email);

                var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password, LModel.RememberMe, lockoutOnFailure: true);
				if (identityResult.Succeeded)
				{
                    var authToken = Guid.NewGuid().ToString();
                    HttpContext.Session.SetString("AuthToken", authToken);
                    Response.Cookies.Append("AuthToken", authToken);

                    user.Logs += 1;
					await userManager.UpdateAsync(user);
                    return RedirectToPage("Index");
				}

                if (identityResult.IsLockedOut)
                {
                    ModelState.AddModelError("", "Account locked out. Please try again later.");
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
using AppSec_Assignment.Model;
using AppSec_Assignment.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Claims;

namespace AppSec_Assignment.Pages
{
    public class RegisterModel : PageModel
    {
        private UserManager<NewUser> userManager { get; }
        private SignInManager<NewUser> signInManager { get; }
        [BindProperty]
        public Register RModel { get; set; }
        public RegisterModel(UserManager<NewUser> userManager, SignInManager<NewUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        public void OnGet(){}

		[ValidateAntiForgeryToken]
		public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var protectYourData = DataProtectionProvider.Create("EncryptData");
                var protec = protectYourData.CreateProtector("LfNRsWjR4ylyZLRECcFh0");
                var userAlreadyExists = await userManager.FindByEmailAsync(RModel.Email);
                if (userAlreadyExists != null)
                {
                    ModelState.AddModelError("", "Email address is already registered.");
                    return Page();
                }

                var user = new NewUser()
                {
                    UserName = RModel.Email,
                    Email = RModel.Email,
                    FirstName = protec.Protect(RModel.FirstName),
                    LastName = protec.Protect(RModel.LastName),
                    Gender = protec.Protect(RModel.Gender),
                    NRIC = protec.Protect(RModel.NRIC),
                    DateofBirth = RModel.DateOfBirth,
                    WAI = RModel.WAI,
                };

                var result = await userManager.CreateAsync(user, RModel.Password);
                if (result.Succeeded)
                {
					var authToken = Guid.NewGuid().ToString();
					HttpContext.Session.SetString("AuthToken", authToken);
					Response.Cookies.Append("AuthToken", authToken);
					//await signInManager.SignInAsync(user, false);
                    return RedirectToPage("Login");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return Page();
        }
    }
}
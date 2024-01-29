using AppSec_Assignment.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AppSec_Assignment.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string NRIC { get; set; }
        public DateTime DOB { get; set; }

        public int LOG { get; set; }
        public string WAI { get; set; }

        private readonly ILogger<IndexModel> _logger;
        private readonly UserManager<NewUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly SignInManager<NewUser> signInManager;

        public IndexModel(ILogger<IndexModel> logger, UserManager<NewUser> userManager, IHttpContextAccessor contextAccessor, SignInManager<NewUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            this.signInManager = signInManager;
        }

        public async Task OnGetAsync()
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

            if (User.Identity.IsAuthenticated)
            {
                /*var user = await _userManager.GetUserAsync(User);

                // Check if the user's session identifier matches the stored one
                var sessionAuthToken = _contextAccessor.HttpContext.Session.GetString("AuthToken");
                var cookieAuthToken = _contextAccessor.HttpContext.Request.Cookies["AuthToken"];

                if (sessionAuthToken != null && cookieAuthToken != null && sessionAuthToken == cookieAuthToken)
                {

                }
                else
                {
                    Response.Redirect("/Login");
                }*/

                var user = await _userManager.GetUserAsync(User);
                var protectYourData = DataProtectionProvider.Create("EncryptData");
                var protec = protectYourData.CreateProtector("LfNRsWjR4ylyZLRECcFh0");

                FirstName = protec.Unprotect(user.FirstName);
                LastName = protec.Unprotect(user.LastName);
                Gender = protec.Unprotect(user.Gender);
                Email = user.Email;
                NRIC = protec.Unprotect(user.NRIC);
                DOB = user.DateofBirth;
                LOG = user.Logs;
                WAI = user.WAI;

                _contextAccessor.HttpContext.Session.SetString("FirstName", FirstName);
                _contextAccessor.HttpContext.Session.SetString("LastName", LastName);
                _contextAccessor.HttpContext.Session.SetString("Gender", Gender);
                _contextAccessor.HttpContext.Session.SetString("Email", Email);
                _contextAccessor.HttpContext.Session.SetString("NRIC", NRIC);
                _contextAccessor.HttpContext.Session.SetString("DOB", DOB.ToString("MM/dd/yyyy"));
                _contextAccessor.HttpContext.Session.SetInt32("LOG", LOG);
                _contextAccessor.HttpContext.Session.SetString("WAI", WAI);
            }
        }
    }
}
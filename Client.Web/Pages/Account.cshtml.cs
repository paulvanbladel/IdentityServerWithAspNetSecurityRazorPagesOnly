using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Client.Web.Pages
{
    public class AccountModel : PageModel
    {
        public AccountModel()
        {

        }
        public void OnGet()
        {

        }
        public IActionResult OnGetTest(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            return LocalRedirect(returnUrl);
        }
        public IActionResult OnGetChallenge(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            var result = Challenge(new AuthenticationProperties { RedirectUri = returnUrl });
            return result;
        }

        public IActionResult OnGetLogOff()
        {
            var schemas = new[]
             {
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme
            };

            return SignOut( schemas);
        }
    }
}
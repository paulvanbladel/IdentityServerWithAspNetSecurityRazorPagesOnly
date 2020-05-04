using IdentityServer4.Services;
using Ids.Web.Data;
using Ids.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Ids.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly IEventService _events;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        [BindProperty(SupportsGet = true)]
        public string LogoutId { get; set; }

        public LogoutModel(IEventService events,
            IIdentityServerInteractionService interaction,
            SignInManager<ApplicationUser> signInManager,
            ILogger<LogoutModel> logger)
        {
            this._events = events;
            this._interaction = interaction;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnGet()
        {
            // build a model so the logout page knows what to display
            var vm = await BuildLogoutViewModelAsync(this.LogoutId);

            if (vm.ShowLogoutPrompt == false)
            {
                // if the request for logout was properly authenticated from IdentityServer, then
                // we don't need to show the prompt and can just log the user out directly.
                return GoToLoggedOutPage(this.LogoutId);
            }

            return Page();
        }
        public IActionResult OnPost(string returnUrl = null)
        {
            return GoToLoggedOutPage(this.LogoutId);
        }

        private IActionResult GoToLoggedOutPage(string logoutId)
        {
            return RedirectToPage("LoggedOut", new { LogoutId = logoutId });
        }

        private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

            if (User?.Identity.IsAuthenticated != true)
            {
                // if the user is not authenticated, then just show logged out page
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            var context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            return vm;
        }



    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Ids.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using IdentityServer4.Services;
using Ids.Web.Models;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Events;
using Microsoft.AspNetCore.Authentication;

namespace Ids.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoggedOutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoggedOutModel> _logger;
        private readonly IEventService _events;
        private readonly IIdentityServerInteractionService _interaction;


        [BindProperty(SupportsGet = true)]
        public string LogoutId { get; set; }

        public LoggedOutViewModel LoggedOutViewModelData { get; set; }
        public LoggedOutModel(SignInManager<ApplicationUser> signInManager, ILogger<LoggedOutModel> logger,
             IEventService events,
             IIdentityServerInteractionService interaction)
        {
            _signInManager = signInManager;
            _logger = logger;
            this._events = events;
            this._interaction = interaction;
        }

        public async Task<IActionResult> OnGet()
        {
            this.LoggedOutViewModelData = await BuildLoggedOutViewModelAsync(this.LogoutId);

            if (User?.Identity.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await _signInManager.SignOutAsync();

                // raise the logout event
                await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
            }

            // check if we need to trigger sign-out at an upstream identity provider
            if (this.LoggedOutViewModelData.TriggerExternalSignout)
            {
                // build a return URL so the upstream provider will redirect back
                // to us after the user has logged out. this allows us to then
                // complete our single sign-out processing.
                string url = Url.Action("Logout", new { logoutId = this.LoggedOutViewModelData.LogoutId });

                // this triggers a redirect to the external provider for sign-out
                return SignOut(new AuthenticationProperties { RedirectUri = url }, this.LoggedOutViewModelData.ExternalAuthenticationScheme);
            }

            return Page();
        }



        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
                SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId
            };

            if (User?.Identity.IsAuthenticated == true)
            {
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServer4.IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
                    if (providerSupportsSignout)
                    {
                        if (vm.LogoutId == null)
                        {
                            // if there's no current logout context, we need to create one
                            // this captures necessary info from the current logged in user
                            // before we signout and redirect away to the external IdP for signout
                            vm.LogoutId = await _interaction.CreateLogoutContextAsync();
                        }

                        vm.ExternalAuthenticationScheme = idp;
                    }
                }
            }
            return vm;
        }
        
    }
}

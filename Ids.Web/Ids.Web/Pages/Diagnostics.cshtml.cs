using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ids.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ids.Web.Pages
{
    [SecurityHeaders]
    [Authorize]
    public class DiagnosticsModel : PageModel
    {
        public DiagnosticsViewModel DiagnosticsViewModel { get; set; }
        public async Task<IActionResult> OnGet()
        {
            var localAddresses = new string[] { "127.0.0.1", "::1", HttpContext.Connection.LocalIpAddress.ToString() };
            if (!localAddresses.Contains(HttpContext.Connection.RemoteIpAddress.ToString()))
            {
                return new NotFoundResult();
            }

            this.DiagnosticsViewModel = new DiagnosticsViewModel(await HttpContext.AuthenticateAsync());
            return Page();
        }
    }
}
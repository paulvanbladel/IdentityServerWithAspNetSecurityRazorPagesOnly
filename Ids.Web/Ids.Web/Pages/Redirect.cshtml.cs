using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ids.Web.Pages
{
    public class RedirectModel : PageModel
    {

        [BindProperty(SupportsGet = true, Name = "redirectUrl")]
        public string RedirectUrl { get; set; }
        public void OnGet(string r) { }
    }
}
﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ids.Web.Models
{
    public class ConsentInputModel
    {
        public string Button { get; set; }
        public IEnumerable<string> ScopesConsented { get; set; }
        public bool RememberConsent { get; set; }
        public string ReturnUrl { get; set; }
    }
}

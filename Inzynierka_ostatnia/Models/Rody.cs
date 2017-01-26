using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages.Html;

namespace Inzynierka_ostatnia.Models
{
    public class Rody
    {
        public string sel_rod { get; set; }
        public IEnumerable<System.Web.Mvc.SelectListItem> sel_rodList
        {
            get
            {
                return new List<System.Web.Mvc.SelectListItem>
                {
                    new System.Web.Mvc.SelectListItem { Text="W11", Value="W11", Selected=true},
                    new System.Web.Mvc.SelectListItem { Text="W33", Value="W33"}

                };
            }
        }
    }
}
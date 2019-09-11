using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocalShowsOnly.Models.ViewModels
{
    public class EventVenueViewModel
    {
        public List<SelectListItem> VenuesSelectListItem { get; set; }
        public Event Event { get; set; }

    }
}

using ArrayCMS.Models;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models;

namespace ArrayCMS.Components
{
	public class BookingFormViewComponent : ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync()
		{
			return View("~/Views/Partials/Forms/BookingForm.cshtml", new BookingFormViewModel());
		}
	}
}



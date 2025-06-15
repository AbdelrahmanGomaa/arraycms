using Microsoft.AspNetCore.Mvc;
using ArrayCMS.Models;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;
using Microsoft.AspNetCore.Identity;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Models.Email;
using Umbraco.Cms.Core.Mail;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Web.Common;
using ArrayCMS.Models;
using Umbraco.Extensions;

namespace ArrayCMS.Controllers
{
	public class BookingFormController : SurfaceController
	{
		private readonly GlobalSettings _globalSettings;
		private readonly IEmailSender _emailSender;
		private readonly UmbracoHelper _umbracoHelper;
		private readonly IConfiguration _configuration;

		public BookingFormController(
			IUmbracoContextAccessor umbracoContextAccessor,
			IUmbracoDatabaseFactory databaseFactory,
			ServiceContext services,
			AppCaches appCaches,
			IProfilingLogger profilingLogger,
			IPublishedUrlProvider publishedUrlProvider,
			IOptions<GlobalSettings> globalSettings,
			IEmailSender emailSender,
			UmbracoHelper umbracoHelper,
			IConfiguration configuration)
			: base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
		{
			//_globalSettings = globalSettings;
			_globalSettings = globalSettings.Value;
			_emailSender = emailSender;
			_umbracoHelper = umbracoHelper;
			_configuration = configuration;
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
			public async Task<IActionResult> SubmitAsync(BookingFormViewModel model)
		{
			if (ModelState.IsValid)
			{
				var contentService = Services.ContentService;
				//var parentId = Guid.Parse("eb2b4c9e-a9be-43b9-abf3-8be59e108405");
				// production
				//var parentId = Guid.Parse("eb2b4c9e-a9be-43b9-abf3-8be59e108405");
				var parentId = Guid.Parse(_configuration["forms:bookingid"]);
				var entry = contentService.Create(model.Email, parentId, "bookingItem");
				entry.SetValue("fullName", model.FullName);
				entry.SetValue("email", model.Email);
				entry.SetValue("phoneNumber", model.PhoneNumber);
				entry.SetValue("courses", model.Courses.FirstOrDefault().ToString());
				entry.SetValue("position", model.Position.FirstOrDefault().ToString());

				var offlinePositionValue = model.OfflinePosition != null && model.OfflinePosition.Any()
					? model.OfflinePosition.FirstOrDefault().ToString() : null;

				var onlinePositionValue = model.OnlinePosition != null && model.OnlinePosition.Any()
					? model.OnlinePosition.FirstOrDefault().ToString() : null;

				entry.SetValue("offlinePosition", offlinePositionValue);
				entry.SetValue("onlinePosition", onlinePositionValue);
			
				entry.SetValue("about", model.About);

					

				contentService.SaveAndPublish(entry);


				ModelState.Clear();
				model = new BookingFormViewModel(); // Reset form fields to default/empty values

				// Work with form data here
				var from = _globalSettings.Smtp?.From;
				var to = _globalSettings.Smtp?.From;

				var message = "Full name " + model.FullName;
				message += "<br> Phone " + model.PhoneNumber;
				message += "<br> E-mail " + model.Email;
				message += "<br> Course " + model.Courses.FirstOrDefault().ToString();

				message += "<br> Position " + model.Position.FirstOrDefault().ToString();

				if (model.OfflinePosition != null && model.OfflinePosition.Any())
				{
					message += "<br> Offline Appoiment " + model.OfflinePosition.FirstOrDefault().ToString();
				}
				else
				{
					message += "<br> Offline Appoiment Not Selected";
				}


				if (model.OnlinePosition != null && model.OnlinePosition.Any())
				{
					message += "<br> Online Appoiment " + model.OnlinePosition.FirstOrDefault().ToString();
				}
				else
				{
					message += "<br> Online Appoiment Not Selected";
				}


				message += "<br> About " + model.About;

				var subject = "Booking Form - " + model.About;

				var mailMessage = new EmailMessage(from, to, subject, message, true);

				await _emailSender.SendAsync(mailMessage, Constants.Web.EmailTypes.Notification);

				//return StatusCode(StatusCodes.Status200OK);
				return CurrentUmbracoPage();

			}
			else
			{

				//return StatusCode(StatusCodes.Status400BadRequest);
				return StatusCode(StatusCodes.Status400BadRequest, "An unexpected error occurred. Please try again later.");
			}
		}
	}
}
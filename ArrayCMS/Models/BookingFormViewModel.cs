using System.ComponentModel.DataAnnotations;

namespace ArrayCMS.Models
{
	public class BookingFormViewModel
	{

		public string FullName { get; set; }

		[RegularExpression(@"^[0-9]*$", ErrorMessage = "Phone Number must contain only digits")]
		public string PhoneNumber	 { get; set; }

		[RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid Email Address")]
		public string Email { get; set; }

		public List<string> Courses { get; set; } = new List<string>();
		public List<string> Position { get; set; } = new List<string>();
		public List<string> ? OfflinePosition { get; set; }
		public List<string> ? OnlinePosition { get; set; }

		public string About { get; set; }

	}
}

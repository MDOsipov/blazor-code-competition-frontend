using System.ComponentModel.DataAnnotations;

namespace BlazorApplication.Models
{
	public class Participant
	{
		public int id { get; set; }
		[Required(ErrorMessage = "First name is required field")]
		public string firstName { get; set; }

		[Required(ErrorMessage = "Last name is required field")]
		public string lastName { get; set; }

		[Required(ErrorMessage = "Email is required field")]
		public string email { get; set; }
		public int userId { get; set; }
		public string teamName { get; set; }
		public int? teamId { get; set; }

	}
}

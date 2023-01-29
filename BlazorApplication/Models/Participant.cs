using System.ComponentModel.DataAnnotations;

namespace BlazorApplication.Models
{
	public class Participant
	{
		public int id { get; set; }
		[Required(ErrorMessage = "First name is required field")]
		public string firstName { get; set; } = string.Empty;

		[Required(ErrorMessage = "Last name is required field")]
		public string lastName { get; set; } = string.Empty;

		[Required(ErrorMessage = "Email is required field")]
		public string email { get; set; } = string.Empty;
		public int userId { get; set; }
		public string teamName { get; set; } = string.Empty;
		public int? teamId { get; set; }
        public bool SuccessRequest { get; set; } = false;

    }
}

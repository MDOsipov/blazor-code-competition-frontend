using System.ComponentModel.DataAnnotations;

namespace BlazorApplication.Models
{
	public partial class Task
	{

		public int Id { get; set; }

		[Required(ErrorMessage = "Task name is required field")]
		public string TaskName { get; set; } = null!;

		[Required(ErrorMessage = "Task description is required field")]
		public string TaskDescription { get; set; } = null!;

		[Required(ErrorMessage = "Task content is required field")]
		public string TaskContent { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a category")]
        public int TaskCategoryId { get; set; }

		public TimeSpan Timeframe { get; set; }

		[Range(1, double.MaxValue, ErrorMessage = "Value for the point can't be lower than 1")]
		public int Points { get; set; }
        public string TaskCategoryName { get; set; }

		//public DateTime CreateDate { get; set; }
		//public DateTime UpdateDate { get; set; }
		//public int CreateUserId { get; set; }
		//public int UpdateUserId { get; set; }
		//public int StatusId { get; set; }

	}
}




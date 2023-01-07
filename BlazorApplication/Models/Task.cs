namespace BlazorApplication.Models
{
	public partial class Task
	{

		public int Id { get; set; }
		public string TaskName { get; set; } = null!;
		public string TaskDescription { get; set; } = null!;
		public string TaskContent { get; set; } = null!;
		public int TaskCategoryId { get; set; }
		public TimeSpan? Timeframe { get; set; }
		public int Points { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime UpdateDate { get; set; }
		public int CreateUserId { get; set; }
		public int UpdateUserId { get; set; }
		public int StatusId { get; set; }

	}
}




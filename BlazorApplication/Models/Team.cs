namespace BlazorApplication.Models
{
	public class Team
	{
		public int Id { get; set; }
		public string TeamName { get; set; } = null!;
		public string TeamLeaderEmail { get; set; } = null!;
		public int TotalPoints { get; set; }
		public string CompetitionName { get; set; } = null!;

        public byte[]? IconImage { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime UpdateDate { get; set; }
		public int CreateUserId { get; set; }
		public int UpdateUserId { get; set; }
		public int StatusId { get; set; }
	}
}

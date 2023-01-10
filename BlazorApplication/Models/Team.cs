using System.ComponentModel.DataAnnotations;

namespace BlazorApplication.Models
{
	public class Team
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Team name is required field")]
		public string TeamName { get; set; } = null!;
		public int TeamLeaderId { get; set; }
        public string TeamLeaderEmail { get; set; } = null!;

		[Required(ErrorMessage = "Total points is required field")]
		public int TotalPoints { get; set; }
		public string CompetitionName { get; set; } = null!;
        public int CompetitionId { get; set; }


        public byte[]? IconImage { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime UpdateDate { get; set; }
		public int CreateUserId { get; set; }
		public int UpdateUserId { get; set; }
		public int StatusId { get; set; }
	}
}

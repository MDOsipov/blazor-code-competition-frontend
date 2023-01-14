using System.ComponentModel.DataAnnotations;

namespace BlazorApplication.Models
{
    public class Competition
    {
        public int id { get; set; }

		[Required(ErrorMessage = "Competition name is required field")]
		public string CompetitionName { get; set; } = null!;
        public int CompetitionStatusId { get; set; }
        public int CompetitionAdministratorId { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Value for the max tasks per group can't be lower than 1")]
        public int maxTaskPerGroups { get; set; }
        public int competitionStatus { get; set; }
        public string competitionStatusName { get; set; }
		public string? HashCode { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public int CreateUserId { get; set; }
        public int UpdateUserId { get; set; }
        public int StatusId { get; set; }
    }
}

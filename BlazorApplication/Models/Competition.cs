namespace BlazorApplication.Models
{
    public class Competition
    {
        public int Id { get; set; }
        public string CompetitionName { get; set; } = null!;
        public int CompetitionAdministratorId { get; set; }
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

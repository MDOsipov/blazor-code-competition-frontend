﻿namespace BlazorApplication.Models
{
	public class TaskToTeam
	{
		public int Id { get; set; }
		public int TeamId { get; set; }
		public int TaskId { get; set; }
		public int TaskStatusId { get; set; }
		public int ParticipantIdForTask { get; set; }
		public int? ReachedScore { get; set; }
        public string? GithubUrl { get; set; }
        public string? SubmitterComment { get; set; }
        public string? CompetitionAdminComment { get; set; }
        public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
    }
}

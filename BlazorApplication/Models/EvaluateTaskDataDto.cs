using System.ComponentModel.DataAnnotations;

namespace BlazorApplication.Models
{
    public class EvaluateTaskDataDto
    {
        public int TaskId { get; set; } 
        public int TeamId { get; set; }
        public int CompetitionId { get; set; }
        public int TaskStatusId { get; set; }
        public int Score { get; set; } = 0;
        public string Comment { get; set; } = "";
    }
}

namespace BlazorApplication.Models
{
	public class CompetitionStatus
	{
		public int Id { get; set; }
		public string StatusName { get; set; } = null!;
        public bool? SuccessRequest { get; set; } = false;
    }
}

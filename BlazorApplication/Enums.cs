namespace BlazorApplication
{
	public class Enums
	{
		public enum TaskStatus
		{
			InProgress = 1,
			Success = 2,
			Fail = 3,
			PartialSuccess = 4
		}

		public enum TaskCategory
		{
			Easy = 1,
			Normal = 2,
			Hard = 3,
			Expert = 4
		}

        public enum Status
        {
            Active = 1,
            NotActive = 2
        }
    }
}

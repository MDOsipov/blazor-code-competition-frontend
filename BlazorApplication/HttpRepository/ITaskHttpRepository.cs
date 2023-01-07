using BlazorApplication.Features;

namespace BlazorApplication.HttpRepository
{
	public interface ITaskHttpRepository
	{
		Task<PagingResponse<Models.Task>> GetTasks(TaskParameters taskParameters);
	}
}

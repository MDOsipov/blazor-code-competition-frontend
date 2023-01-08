using BlazorApplication.HttpRepository;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace BlazorApplication.Pages
{
	public partial class CreateTask
	{
		private Models.Task _task = new Models.Task();
		private string _timeFrameMode = "1";
		private Enums.TaskCategory _taskCategory = Enums.TaskCategory.Easy;
		private SuccessNotification _notification;

		[Inject]
		public ITaskHttpRepository TaskRepo { get; set; }

		private async void Create()
		{
			_task.CreateDate = DateTime.Now;	
			_task.UpdateDate = DateTime.Now;
			_task.CreateUserId = 1;
			_task.UpdateUserId = 1;
			_task.StatusId = 1;
			_task.TaskCategoryId = (int)_taskCategory;

			switch (_timeFrameMode)
			{
				case "1":
					_task.Timeframe = new TimeSpan(0, 30, 0);
					break;
				case "2":
					_task.Timeframe = new TimeSpan(1, 0, 0);
					break;
				case "3":
					_task.Timeframe = new TimeSpan(1, 30, 0);
					break;
				case "4":
					_task.Timeframe = new TimeSpan(2, 0, 0);
					break;
			}
			
			await TaskRepo.CreateTask(_task);
			_notification.Show();
		}
	}
}

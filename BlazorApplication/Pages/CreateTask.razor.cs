using BlazorApplication.HttpRepository;
using BlazorApplication.Models;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace BlazorApplication.Pages
{
	public partial class CreateTask
	{
        public List<TaskCategory> TaskCategories { get; set; } = new List<TaskCategory>();
        private Models.Task _task { get; set; } = new Models.Task();
		private string _taskCategory { get; set;}
		private SuccessNotification? _notification;
        private string _time { get; set; } = string.Empty;


        [Inject]
		public ITaskHttpRepository TaskRepo { get; set; }

        [Inject]
        public ITaskCategoryHttpRepository TaskCategoryRepo { get; set; }

        protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
            await GetTaskCategories();
        }

        private async void Create()
		{

			_task.TaskCategoryId = TaskCategories.Where(tc => tc.CategoryName == _taskCategory).Select(tc => tc.Id).FirstOrDefault();
            Console.WriteLine(_task.TaskCategoryId);

            if (_time.Contains('.'))
            {
                _time = _time.Replace('.', ':');
            }
            if (_time.Contains(':'))
            {
                string[] numbers = _time.Split(':');
                if (numbers.Length == 3)
                {

                    try
                    {
                        if (Convert.ToInt32(numbers[0]) < 25)
                        {
                            _task.Timeframe = new TimeSpan(Convert.ToInt32(numbers[0]), Convert.ToInt32(numbers[1]), Convert.ToInt32(numbers[2]));
                        }
                        else
                        {
                            return;
                        }

                    }
                    catch
                    {
                        return;
                    }

                }
                else if (numbers.Length == 2)
                {
                    try
                    {
                        if (Convert.ToInt32(numbers[0]) < 25)
                        {
                            _task.Timeframe = new TimeSpan(Convert.ToInt32(numbers[0]), Convert.ToInt32(numbers[1]), 0);
                        }
                        else
                        {
                            return;
                        }
                    }
                    catch
                    {
                        return;
                    }
                }
            }
            else
            {
                try
                {
                    _task.Timeframe = new TimeSpan(0, Convert.ToInt32(_time), 0);
                }
                catch
                {
                    return;
                }
            }

            await TaskRepo.CreateTask(_task);
			_notification.Show();
		}

        protected async System.Threading.Tasks.Task GetTaskCategories()
        {
            TaskCategories = await TaskCategoryRepo.GetTaskCategory();
        }
	}
}

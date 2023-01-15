using BlazorApplication.Interfaces;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;
using System.Text.Json;

namespace BlazorApplication.Pages
{
    public partial class UpdateTask : ComponentBase
    {
        private Models.Task _task { get; set; }
        private string _timeFrameMode { get; set; }
        private string _taskCategory { get; set; }
        private List<Models.TaskCategory> TaskCategories { get; set; }


        private SuccessNotification _notification;

        [Inject]
        public ITaskHttpRepository TaskRepo { get; set; }

        [Inject]
        public ITaskCategoryHttpRepository TaskCategoryRepo { get; set; }

        [Parameter]
        public string Id { get; set; }

        protected async override Task OnInitializedAsync()
        {
            Console.WriteLine("Update task created with id = " + Id);

            _task = await TaskRepo.GetTaskById(Id);

            _timeFrameMode = _task.Timeframe.ToString();

            await GetTaskCategories();

            _taskCategory = TaskCategories.Where(tc => tc.Id == _task.TaskCategoryId).Select(tc => tc.CategoryName).FirstOrDefault();

        }

		private async Task Update()
        {
            _task.TaskCategoryId = TaskCategories.Where(tc => tc.CategoryName == _taskCategory).Select(tc => tc.Id).FirstOrDefault();

            if (_timeFrameMode.Contains('.'))
            {
                _timeFrameMode = _timeFrameMode.Replace('.', ':');
            }
            if (_timeFrameMode.Contains(':'))
            {
                string[] numbers = _timeFrameMode.Split(':');
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
                    _task.Timeframe = new TimeSpan(0, Convert.ToInt32(_timeFrameMode), 0);
                }
                catch
                {
                    return;
                }
            }

            await TaskRepo.UpdateTask(_task);
            _notification.Show();
        }

        protected async Task GetTaskCategories()
        {
            TaskCategories = await TaskCategoryRepo.GetTaskCategory();
        }

    }
}

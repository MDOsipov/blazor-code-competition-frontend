using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace BlazorApplication.Pages
{
    public partial class CreateTask
	{
        private List<Models.TaskCategory> TaskCategories { get; set; } = new List<Models.TaskCategory>();
        private Models.Task _task { get; set; } = new Models.Task();

		private SuccessNotification? _notification;
        private string _time { get; set; } = string.Empty;
        private ErrorBoundary? errorBoundary;

        [Inject]
		public ITaskHttpRepository TaskRepo { get; set; }

        [Inject]
        public ITaskCategoryHttpRepository TaskCategoryRepo { get; set; }

        protected async override Task OnInitializedAsync()
        {
            _task.TaskCategoryId = -1;
            await GetTaskCategories();
        }

        private async void Create()
		{

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

            try
            {
                await TaskRepo.CreateTask(_task);
                _notification.Show();
            }
            catch (Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while creating a new task!", ex);
            }

        }

        protected async Task GetTaskCategories()
        {
            TaskCategoryParameters taskCategoryParameters = new TaskCategoryParameters()
            {
                switchOff = true
            };

            try
            {
                var pagingResponse = await TaskCategoryRepo.GetTaskCategory(taskCategoryParameters);
                TaskCategories = pagingResponse.Items;
            }
            catch(Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while getting a list of task categories!", ex);
            }

        }
        protected override void OnParametersSet()
        {
            errorBoundary?.Recover();
        }
        private void ResetError()
        {
            errorBoundary?.Recover();
        }
    }
}

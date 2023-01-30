using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Text.Json;

namespace BlazorApplication.Pages
{
    public partial class UpdateTask : ComponentBase
    {
        private Models.Task _task { get; set; }
        private string _timeFrameMode { get; set; }
        private string _taskCategory { get; set; }
        private List<Models.TaskCategory> TaskCategories { get; set; }
        private ErrorBoundary? errorBoundary;

        private SuccessNotification _notification;

        [Inject]
        public ITaskHttpRepository TaskRepo { get; set; }

        [Inject]
        public ITaskCategoryHttpRepository TaskCategoryRepo { get; set; }

        [Inject]
        public ILogger<UpdateTask> Logger { get; set; }

        [Parameter]
        public string Id { get; set; }

        protected async override Task OnInitializedAsync()
        {
            await GetTask();
            await GetTaskCategories();
            _taskCategory = TaskCategories.Where(tc => tc.Id == _task.TaskCategoryId).Select(tc => tc.CategoryName).FirstOrDefault();
        }

        private async Task GetTask()
        {
            Logger.LogInformation("Get task method is called");
            try
            {
                _task = await TaskRepo.GetTaskById(Id);
                _timeFrameMode = _task.Timeframe.ToString();
                Logger.LogInformation($"Success. Task: {JsonSerializer.Serialize(_task)}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while deleting participants!", ex);
            }
        }
		private async Task Update()
        {
            Logger.LogInformation("Update method is called");
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

            try
            {
                await TaskRepo.UpdateTask(_task);
                Logger.LogInformation($"Success. The task is updated");
                _notification.Show();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while updating the task", ex);
            }
        }

        protected async Task GetTaskCategories()
        {
            Logger.LogInformation("Get task categories method is called");
            TaskCategoryParameters taskCategoryParameters = new TaskCategoryParameters()
            {
                switchOff = true
            };

            try
            {
                var pagingResponse = await TaskCategoryRepo.GetTaskCategory(taskCategoryParameters);
                TaskCategories = pagingResponse.Items;
                Logger.LogInformation($"Success. Task categories: {JsonSerializer.Serialize(TaskCategories)}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while getting a list of task categories", ex);
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

using BlazorApplication.HttpRepository;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;
using System.Text.Json;

namespace BlazorApplication.Pages
{
    public partial class UpdateTask : ComponentBase
    {
        private Models.Task _task = new Models.Task();
        private Enums.TaskCategory _taskCategory = Enums.TaskCategory.Easy;
        private string _timeFrameMode = "1";


        private SuccessNotification _notification;

        [Inject]
        public ITaskHttpRepository TaskRepo { get; set; }

        [Parameter]
        public string Id { get; set; } = "";

        protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
            Console.WriteLine("Update task created with id = " + Id);
            _task = await TaskRepo.GetTaskById(Id);
            _taskCategory = (Enums.TaskCategory)_task.TaskCategoryId;
            if (_task.Timeframe.Hours == 0 && _task.Timeframe.Minutes == 30)
            {
                _timeFrameMode = "1";
            }
            else if (_task.Timeframe.Hours == 1 && _task.Timeframe.Minutes == 0)
            {
                _timeFrameMode = "2";
            }
            else if (_task.Timeframe.Hours == 1 && _task.Timeframe.Minutes == 30)
            {
                _timeFrameMode = "3";
            }
            else if (_task.Timeframe.Hours == 2 && _task.Timeframe.Minutes == 0)
            {
                _timeFrameMode = "4";
            }
            Console.WriteLine("Task returned:");
			Console.WriteLine(JsonSerializer.Serialize(_task));
		}

		private async Task Update()
        {
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

            await TaskRepo.UpdateTask(_task);
            _notification.Show();
        }

    }
}

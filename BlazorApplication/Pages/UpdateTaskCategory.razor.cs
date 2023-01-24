using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;
using System.Text.Json;

namespace BlazorApplication.Pages
{
    public partial class UpdateTaskCategory
    {
        private TaskCategory _taskCategory = new TaskCategory();
        private SuccessNotification _notification;

        public List<Team> teamList { get; set; } = new List<Team>();

        [Inject]
        public ITaskCategoryHttpRepository TaskCategoryRepo { get; set; }

        [Parameter]
        public string Id { get; set; } = "";

        protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
            _taskCategory = await TaskCategoryRepo.GetTaskCategoryById(Id);
            Console.WriteLine("Got task category object: ");
            Console.WriteLine(JsonSerializer.Serialize(_taskCategory));
        }

        private async System.Threading.Tasks.Task Update()
        {
            await TaskCategoryRepo.UpdateTaskCategory(_taskCategory);
            _notification.Show();
        }
    }
}

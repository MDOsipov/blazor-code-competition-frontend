using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorApplication.Pages
{
    public partial class CreateTaskCategory
    {
        private TaskCategory _taskCategory = new TaskCategory();

        private SuccessNotification _notification;

        [Inject]
        public ITaskCategoryHttpRepository TaskCategoryRepo { get; set; }

        protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
          
        }

        private async void Create()
        {
            await TaskCategoryRepo.CreateTaskCategory(_taskCategory);
            _notification.Show();
        }
    }
}

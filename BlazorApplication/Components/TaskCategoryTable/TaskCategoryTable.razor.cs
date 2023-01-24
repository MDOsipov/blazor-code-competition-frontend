using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorApplication.Components.TaskCategoryTable
{
    public partial class TaskCategoryTable
    {
        [Inject]
        public IJSRuntime Js { get; set; }

        [Parameter]
        public List<Models.TaskCategory> TaskCategories { get; set; }

        [Parameter]
        public EventCallback<int> OnDeleted { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        private void RedirectToUpdate(int id)
        {
            var url = Path.Combine("/updateTaskCategory/", id.ToString());
            Console.WriteLine("Url - " + url);
            NavigationManager.NavigateTo(url);
        }

        private async Task Delete(int id)
        {
            var taskCategory = TaskCategories.FirstOrDefault(p => p.Id.Equals(id));
            var confirmed = await Js.InvokeAsync<bool>("confirm", $"Are you sure you want to delete {taskCategory.CategoryName} task?");

            if (confirmed)
            {
                await OnDeleted.InvokeAsync(id);
            }
        }
    }
}

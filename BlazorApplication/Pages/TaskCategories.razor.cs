using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorApplication.Pages
{
    public partial class TaskCategories
    {
        public List<Models.TaskCategory> TaskCategoryList { get; set; } = new List<Models.TaskCategory>();
        public MetaData MetaData { get; set; } = new MetaData();
        private TaskCategoryParameters _taskCategoryParameters = new TaskCategoryParameters();

        [Inject]
        public ITaskCategoryHttpRepository TaskCategoryRepo { get; set; }

        protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
            await GetTaskСategories();
        }

        private async System.Threading.Tasks.Task SelectedPage(int page)
        {
            _taskCategoryParameters.PageNumber = page;
            await GetTaskСategories();
        }

        protected async System.Threading.Tasks.Task GetTaskСategories()
        {
            var pagingResponse = await TaskCategoryRepo.GetTaskCategory(_taskCategoryParameters);
            TaskCategoryList = pagingResponse.Items;
            MetaData = pagingResponse.MetaData;
        }

        
        private async System.Threading.Tasks.Task DeleteTaskCategory(int id)
        {
            await TaskCategoryRepo.DeleteTaskCategory(id);
            _taskCategoryParameters.PageNumber = 1;
            await GetTaskСategories();
        }
        
    }
}

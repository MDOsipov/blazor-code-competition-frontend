using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorApplication.Pages
{
    public partial class TaskCategories
    {
        public List<Models.TaskCategory> TaskCategoryList { get; set; } = new List<Models.TaskCategory>();
        public MetaData MetaData { get; set; } = new MetaData();
        private TaskCategoryParameters _taskCategoryParameters = new TaskCategoryParameters();
        private ErrorBoundary? errorBoundary;

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
            try
            {
                var pagingResponse = await TaskCategoryRepo.GetTaskCategory(_taskCategoryParameters);
                TaskCategoryList = pagingResponse.Items;
                MetaData = pagingResponse.MetaData;
            }
            catch(Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while getting a list of task categories!", ex);
            }
        }

        
        private async System.Threading.Tasks.Task DeleteTaskCategory(int id)
        {
            try
            {
                await TaskCategoryRepo.DeleteTaskCategory(id);
                _taskCategoryParameters.PageNumber = 1;
            }
            catch (Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while deleting a task category!", ex);
            }
            await GetTaskСategories();
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

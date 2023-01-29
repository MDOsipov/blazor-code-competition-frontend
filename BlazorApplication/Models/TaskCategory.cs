using System.ComponentModel.DataAnnotations;

namespace BlazorApplication.Models
{
    public class TaskCategory
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required field")]
        public string CategoryName { get; set; }

        [Required(ErrorMessage = "Category description is required field")]
        public string? CategoryDescription { get; set; }
        public bool? SuccessRequest { get; set; } = false;
    }
}

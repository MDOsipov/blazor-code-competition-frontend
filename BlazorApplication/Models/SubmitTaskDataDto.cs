using System.ComponentModel.DataAnnotations;

namespace BlazorApplication.Models
{
    public class SubmitTaskDataDto
    {
        [Required(ErrorMessage = "Github url is required field")]
        public string GithubUrl { get; set; } = "";

        public string SubmitterComment { get; set; } = "";
    }
}

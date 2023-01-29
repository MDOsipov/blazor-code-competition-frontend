namespace BlazorApplication.Features
{
    public class ResponseWithSuccess<T> where T: class
    {
        public List<T> Items { get; set; }
        public bool SuccessRequest { get; set; } = false;
    }
}

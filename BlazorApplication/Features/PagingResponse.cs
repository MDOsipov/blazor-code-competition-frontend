using BlazorApplication.Models;
using System.Reflection.Metadata.Ecma335;

namespace BlazorApplication.Features
{
	public class PagingResponse<T> where T : class
	{
		public List<T> Items { get; set; }
		public MetaData MetaData { get; set; }
    }
}

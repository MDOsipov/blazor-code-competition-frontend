using BlazorApplication;
using BlazorApplication.HttpRepository;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Http;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient {});

/*
builder.Services.AddHttpClient<TaskHttpRepository>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7192/");
});

builder.Services.AddHttpClient<TeamHttpRepository>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7192/");
});
*/

builder.Services.AddScoped<ITaskHttpRepository, TaskHttpRepository>();
builder.Services.AddScoped<ITeamHttpRepository, TeamHttpRepository>();
builder.Services.AddScoped<ICompetitionHttpRepository, CompetitionHttpRepository>();



await builder.Build().RunAsync();

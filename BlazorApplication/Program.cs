using BlazorApplication;
using BlazorApplication.HttpRepository;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorApplication.Features;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

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
builder.Services.AddScoped<IParticipantHttpRepository, ParticipantHttpRepository>();

builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Auth0", options.ProviderOptions);
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.AdditionalProviderParameters.Add("audience", builder.Configuration["Auth0:Audience"]);
}).AddAccountClaimsPrincipalFactory<ArrayClaimsPrincipalFactory<RemoteUserAccount>>();


await builder.Build().RunAsync();

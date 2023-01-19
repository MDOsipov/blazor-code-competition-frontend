using BlazorApplication;
using BlazorApplication.HttpRepository;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorApplication.Features;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using BlazorApplication.Interfaces;
using Tewr.Blazor.FileReader;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient {});

builder.Services.AddScoped<ITaskHttpRepository, TaskHttpRepository>();
builder.Services.AddScoped<ITeamHttpRepository, TeamHttpRepository>();
builder.Services.AddScoped<ICompetitionHttpRepository, CompetitionHttpRepository>();
builder.Services.AddScoped<IParticipantHttpRepository, ParticipantHttpRepository>();
builder.Services.AddScoped<ITaskCategoryHttpRepository, TaskCategoryHttpRepository>();
builder.Services.AddScoped<IUserHttpRepository, UserHttpRepository>();


builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Auth0", options.ProviderOptions);
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.AdditionalProviderParameters.Add("audience", builder.Configuration["Auth0:Audience"]);
}).AddAccountClaimsPrincipalFactory<ArrayClaimsPrincipalFactory<RemoteUserAccount>>();


await builder.Build().RunAsync();

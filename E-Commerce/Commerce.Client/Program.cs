using Blazored.Toast;
using Commerce.Client;
using MudBlazor.Services;
using Blazored.LocalStorage;
using Cropper.Blazor.Extensions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddMudServices()
    .AddBlazoredLocalStorage()
    .AddBlazoredToast()
    .AddCropper()
    .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
    .AddCommerceServices();

await builder.Build().RunAsync();

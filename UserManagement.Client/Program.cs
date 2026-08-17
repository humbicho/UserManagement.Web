using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using UserManagement.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp =>
    new HttpClient());

builder.Services.AddScoped<IUserService, UserService>();

await builder.Build().RunAsync();
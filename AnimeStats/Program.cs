using AnimeStats;
using AnimeStats.Components;
using AnimeStats.Service;
using Microsoft.EntityFrameworkCore;
using Blazorise;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services
    .AddBlazorise(options =>
    {
        options.Immediate = true;
    });

builder.Services.AddDbContext<DatabaseEFCore>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseConnection")));

builder.Services.AddScoped<IAnimeService, AnimeService>();
builder.Services.AddScoped<IRepository<Anime>, TvRepository>();
builder.Services.AddHttpClient<ISpotifyService, SpotifyService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
using DeviceManager;
using DeviceManager.Data;
using DeviceManager.Repository;
using ElectronNET.API;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddScoped<IProviderRepository, ProviderRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IClaimRepository, ClaimRepository>();
builder.Services.AddSingleton<StateDetails>();
builder.Services.AddDbContext<PatientManagementDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddMudServices();
builder.Services.AddElectron();
builder.WebHost.UseElectron(args);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}


app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
Task.Run(async () => await Electron.WindowManager.CreateWindowAsync());
app.Run();



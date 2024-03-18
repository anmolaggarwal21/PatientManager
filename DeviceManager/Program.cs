using DeviceManager;
using DeviceManager.Data;
using DeviceManager.Repository;
using ElectronNET.API;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Extensions;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddScoped<IProviderRepository, ProviderRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IClaimRepository, ClaimRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBillingRepository, BillingRepository>();
builder.Services.AddScoped<IBillingDetailsRepository, BillingDetailsRepository>();

builder.Services.AddSingleton<StateDetails>();
builder.Services.AddScoped<IClipboardService, ClipboardService>();
builder.Services.AddDbContext<PatientManagementDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<UserDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("UserConnection")));
builder.Services.AddIdentity<Users, IdentityRole>().AddEntityFrameworkStores<UserDBContext>().AddDefaultTokenProviders();
builder.Services.AddMudServices();
builder.Services.AddMudServicesWithExtensions();

// or this to add only the MudBlazor.Extensions but please ensure that this is added after mud servicdes are added. That means after `AddMudServices`
builder.Services.AddMudExtensions();
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
app.UseMudExtensions();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
Task.Run(async () => await Electron.WindowManager.CreateWindowAsync());
app.Run();



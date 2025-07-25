using MyPetMonitor.Services;
using MyPetMonitor.Services.Interfaces;
using MyPetMonitor.Data;
using MyPetMonitor.Hubs;
using Serilog;

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/myPetMonitor-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Agregar Serilog
builder.Host.UseSerilog();

// Blazor Server
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSignalR();

// UI Framework - Blazorise
// TODO: Configurar Blazorise cuando se agreguen los paquetes
// builder.Services
//     .AddBlazorise(options => { options.Immediate = true; })
//     .AddBootstrap5Providers()
//     .AddFontAwesomeIcons();

// Servicios de la aplicación
builder.Services.AddScoped<ICameraService, CameraService>();
builder.Services.AddScoped<IOnvifService, OnvifService>();
builder.Services.AddScoped<IPTZService, PTZService>();

// Repositorio
builder.Services.AddSingleton<ICameraRepository, LiteDbCameraRepository>();

// Background Services
// TODO: Agregar servicios en segundo plano cuando sea necesario
// builder.Services.AddHostedService<CameraDiscoveryService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// SignalR Hub
app.MapHub<VideoStreamHub>("/videohub");

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

Log.Information("🎥 My Pet Monitor starting up...");

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}

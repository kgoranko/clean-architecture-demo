using Application;
using Infrastructure;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Configure Serilog for structured logging (logs to console only - no external dependencies)
builder.Host.UseSerilog((context, loggerConfig) => loggerConfig
    .ReadFrom.Configuration(context.Configuration));

string demoDatabaseConnectionString = builder.Configuration.GetConnectionString("DemoDatabase")
    ?? throw new InvalidOperationException("Connection string 'DemoDatabase' is missing.");

// Register services from each layer following Clean Architecture dependency flow:
// Presentation (Web.App) → Infrastructure → Application → Domain → SharedKernel
builder.Services
    .AddApplication()       // Registers CQRS handlers, validators, application services
    .AddInfrastructure(demoDatabaseConnectionString);   // Registers repositories and infrastructure providers

builder.Services.AddRazorPages();

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

await app.RunAsync();

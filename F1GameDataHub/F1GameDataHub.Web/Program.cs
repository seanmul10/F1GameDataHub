using F1GameDataHub.Web.Services;

var builder = WebApplication.CreateBuilder(args);

var dbConnectionString =
    builder.Configuration["F1_DB_CONNECTION_STRING"]
    ?? Environment.GetEnvironmentVariable("F1_DB_CONNECTION_STRING")
    ?? "Host=localhost;Port=55432;Username=postgres;Password=postgres;Database=postgres";
Environment.SetEnvironmentVariable("F1_DB_CONNECTION_STRING", dbConnectionString);
Environment.SetEnvironmentVariable(
    "F1_UDP_PORT",
    builder.Configuration.GetValue("Listener:UdpPort", 20778).ToString());
Environment.SetEnvironmentVariable("F1_DISABLE_DB", "false");

builder.Services.AddRazorPages();
builder.Services.AddSingleton<JobStatusStore>();
builder.Services.AddSingleton<SessionSummaryService>();
builder.Services.AddSingleton<DumpImportJobService>();
builder.Services.AddSingleton<LiveListenerJobService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

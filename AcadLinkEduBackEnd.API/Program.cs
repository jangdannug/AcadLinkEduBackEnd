using AcadLinkEduBackEnd.Application.Services;
using AcadLinkEduBackEnd.Infrastructure;
using Supabase;

var builder = WebApplication.CreateBuilder(args);

// ✅ Add Controllers
builder.Services.AddControllers();

// ----------------------------
// CORS
// Allow all origins for development (adjust for production)
// ----------------------------
var corsPolicyName = "AllowLocalDev";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicyName, policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ✅ Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ----------------------------
// Initialize Supabase Client
// ----------------------------
var supabaseClient = new Supabase.Client(
    builder.Configuration["SupabaseUrl"],
    builder.Configuration["SupabaseKey"],
    new SupabaseOptions
    {
        AutoRefreshToken = true,
        AutoConnectRealtime = true
    });

await supabaseClient.InitializeAsync();
// Register Supabase client and a simple wrapper service for DI
builder.Services.AddSingleton(supabaseClient);
builder.Services.AddSingleton(new SupabaseService(supabaseClient));

// ✅ Services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ClassService>();
builder.Services.AddScoped<ClassService>();
builder.Services.AddScoped<SubmissionService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<ActivityService>();
// later add:
// builder.Services.AddScoped<ClassService>();
// builder.Services.AddScoped<ActivityService>();

var app = builder.Build();

// ✅ Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // Serve the Swagger UI at application root ("/") so launchBrowser opens it directly
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AcadLinkEduBackEnd API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowLocalDev");

// ✅ Map Controllers
app.MapControllers();

app.Run();
using ExpenseTracker.Components;
using ExpenseTracker.Data;
using ExpenseTracker.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Razor Components with interactive server rendering
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// EF Core + SQLite
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// Dependency Injection
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ToastService>();

var app = builder.Build();

// Ensure SQLite database exists with 0 seed/mock data
using (var scope = app.Services.CreateScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    using var db = factory.CreateDbContext();
    db.Database.EnsureCreated();
    db.Database.ExecuteSqlRaw(@"
        CREATE TABLE IF NOT EXISTS Notifications (
            Id TEXT PRIMARY KEY,
            Title TEXT NOT NULL,
            Message TEXT NOT NULL,
            Type TEXT NOT NULL,
            Timestamp TEXT NOT NULL,
            IsRead INTEGER NOT NULL,
            LinkUrl TEXT
        );");
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

using IssueTracker.Api.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("IssueTracker");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Konfigurera ConnectionStrings:IssueTracker för SQL Server-databasen.");
}

builder.Services.AddControllers();
builder.Services.AddScoped<ITicketService>(_ => new TicketService(connectionString));

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

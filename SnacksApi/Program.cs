var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var snack_schedule = new List<SnackSchedule>
{
    new(DateOnly.Parse("2026-03-16"), "María", "Manzana"),
    new(DateOnly.Parse("2026-03-17"), "Pedro", "Pera"),
    new(DateOnly.Parse("2026-03-18"), "Monica", "Mandarina"),
};

app.MapGet("/snacksSchedule", () => snack_schedule).WithName("GetSnacksSchedule");

app.MapPost("/snacksSchedule", (SnackSchedule snackSchedule) =>
{
    var newSnack = new { snackSchedule.Date, snackSchedule.User, snackSchedule.Snack };
    
    snack_schedule.Add(snackSchedule);

    return Results.Created($"/snacksSchedule/{snackSchedule.Date}", newSnack);
}).WithName("AddSnackToSchedule");

app.Run();

record SnackSchedule(DateOnly Date, string User, string Snack);

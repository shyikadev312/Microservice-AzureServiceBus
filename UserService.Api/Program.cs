using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure the DbContext with SQL Server
builder.Services.AddDbContext<UserContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UserDatabase")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



// Map CRUD endpoints for Users
app.MapGet("/users", async (UserContext db) => await db.Users.ToListAsync());
app.MapGet("/users/{id}", async (int id, UserContext db) =>
    await db.Users.FindAsync(id) is Users user ? Results.Ok(user) : Results.NotFound());

app.MapPost("/users", async (Users user, UserContext db) =>
{
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/users/{user.Id}", user);
});

app.MapPut("/users/{id}", async (int id, Users inputUser, UserContext db) =>
{
    Users user = await db.Users.FindAsync(id);

    if (user is null) return Results.NotFound();

    user.Name = inputUser.Name;
    user.Email = inputUser.Email;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/users/{id}", async (int id, UserContext db) =>
{
    if (await db.Users.FindAsync(id) is Users user)
    {
        db.Users.Remove(user);
        await db.SaveChangesAsync();
        return Results.Ok(user);
    }

    return Results.NotFound();
});

app.Run();

// User entity
record Users
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

// DbContext for User service
class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options) : base(options) { }

    public DbSet<Users> Users { get; set; }
}
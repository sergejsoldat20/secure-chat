using messages_backend.Data;
using messages_backend.Helpers;
using messages_backend.Middleware;
using messages_backend.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");
// Add services to the container.
// var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var connectionString = $"Server={dbHost};Initial Catalog={dbName};User ID=sa;Password={dbPassword};TrustServerCertificate=True;";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString,
    providerOptions => providerOptions.EnableRetryOnFailure()));

// builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.IgnoreNullValues = true);
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddSingleton<AppSettings>();

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICryptoService, CryptoService>();
builder.Services.AddScoped<IMessagesService, MessagesService>();
/// builder.Services.AddScoped<ISteganographyService, SteganographyService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	using (var serviceScope = app.Services
	.GetRequiredService<IServiceScopeFactory>()
	.CreateScope())
	{
		using (var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>())
		{
			context.Database.Migrate();
		}
	}
	app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.Run();

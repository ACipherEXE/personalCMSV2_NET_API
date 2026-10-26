using Microsoft.EntityFrameworkCore;
using personalCMSV2_NET_API.Models;



AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);



var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();


// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("postgressConnection") ?? throw new InvalidOperationException("Connection string 'personalCMSV2_NET_APIContext' not found.")));
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("postgressConnection") ?? throw new InvalidOperationException("Connection string 'YourDbContext' not found."), o => o.ConfigureDataSource(b => b.EnableDynamicJson())));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Local develoment CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevFrontend", policy => policy
        .WithOrigins("http://localhost:5173")// PersonalCMSV2 port
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "personalCMSV2 API v1");
    });
}

app.UseCors("DevFrontend");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

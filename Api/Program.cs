using Logic;
using Logic.Repositories;
using Logic.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<MovieRepository>();
builder.Services.AddTransient<CustomerRepository>();
builder.Services.AddTransient<MovieService>();
builder.Services.AddTransient<CustomerService>();

builder.Services.AddDbContext<OnlineTheatreDbContext>(options =>
    options.UseInMemoryDatabase("OnlineTheater"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseSwaggerUI();

app.Run();